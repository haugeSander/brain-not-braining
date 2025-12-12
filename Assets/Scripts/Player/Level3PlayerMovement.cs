using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using BrainNotBraining.Gameplay;

namespace BrainNotBraining.Player
{
    /// <summary>
    /// Player movement controller for Level 3 (Somatosensory Cortex).
    /// Handles physics-based WASD movement with audio feedback.
    /// Integrates with EcholocationSystem for footstep and manual pulse triggering.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Level3PlayerMovement : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [Tooltip("Movement force applied to rigidbody")]
        public float movementSpeed = 1f;

        [Header("Audio Configuration")]
        [Tooltip("Audio source for movement/footstep sounds")]
        public AudioSource movementAudio;

        [Tooltip("Fade-out duration when stopping movement (seconds)")]
        public float fadeOutDuration = 0.3f;

        [Header("Echolocation Configuration")]
        [Tooltip("Trigger footstep pulse on every step")]
        public bool enableFootstepPulses = true;

        [Tooltip("Interval between footstep pulses (seconds)")]
        public float footstepPulseInterval = 0.5f;

        [Header("Camera Configuration")]
        [Tooltip("Main camera for camera-relative movement (auto-finds if not assigned)")]
        public Camera mainCamera;

        [Tooltip("Use camera-relative movement (WASD relative to camera view)")]
        public bool useCameraRelativeMovement = true;

        [Header("Debug Configuration")]
        [Tooltip("Enable debug teleport with F key")]
        public bool enableDebugTeleport = true;

        [Tooltip("Debug teleport target position")]
        public Vector3 debugTeleportPosition = new Vector3(-17f, 0f, -32f);

        // Internal references
        private Rigidbody rb;
        private EcholocationSystem echolocationSystem;

        // Movement state
        private Vector2 moveInput;
        private bool isMoving = false;

        // Audio state
        private Coroutine fadeRoutine = null;

        // Footstep pulse state
        private float lastFootstepPulseTime = -999f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            echolocationSystem = GetComponent<EcholocationSystem>();

            if (echolocationSystem == null)
            {
                Debug.LogWarning("Level3PlayerMovement: No EcholocationSystem found. Footstep pulses will not work.");
            }

            // Find main camera if not assigned
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.LogError("Level3PlayerMovement: No main camera found! Assign manually or tag camera as MainCamera.");
                }
            }
        }

        private void Update()
        {
            // Debug teleport
            if (enableDebugTeleport && Input.GetKeyDown(KeyCode.F))
            {
                DebugTeleport();
            }
        }

        private void FixedUpdate()
        {
            // Per-axis deadzone so one axis doesn't kill the other
            Vector2 filteredInput = ApplyPerAxisDeadzone(moveInput, 0.12f); // try 0.12; adjust smaller if needed

            // Early-out: no horizontal input → keep only vertical velocity (gravity)
            if (Mathf.Approximately(filteredInput.x, 0f) && Mathf.Approximately(filteredInput.y, 0f))
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
                return;
            }

            // Build basis vectors (camera relative or world)
            Vector3 forward;
            Vector3 right;

            if (useCameraRelativeMovement && mainCamera != null)
            {
                forward = mainCamera.transform.forward;
                forward.y = 0f;
                forward.Normalize();

                right = mainCamera.transform.right;
                right.y = 0f;
                right.Normalize();
            }
            else
            {
                forward = Vector3.forward;
                right = Vector3.right;
            }

            // Compute target horizontal velocity (no premature normalization)
            Vector3 targetHorizontalVel = forward * filteredInput.y * movementSpeed + right * filteredInput.x * movementSpeed;

            // Clamp magnitude so diagonal doesn't exceed movementSpeed
            targetHorizontalVel = Vector3.ClampMagnitude(targetHorizontalVel, movementSpeed);

            // Compose final velocity (preserve vertical velocity)
            Vector3 finalVel = new Vector3(targetHorizontalVel.x, rb.linearVelocity.y, targetHorizontalVel.z);

            rb.linearVelocity = finalVel;

            // Optional debug: uncomment if you want to inspect values
            // Debug.Log($"moveInput: {moveInput}, filtered: {filteredInput}, targetHorVel: {targetHorizontalVel}, finalVel: {finalVel}");
        }

        private Vector2 ApplyPerAxisDeadzone(Vector2 input, float deadzone)
        {
            Vector2 outInput = Vector2.zero;

            // X axis
            if (Mathf.Abs(input.x) >= deadzone)
                outInput.x = input.x;
            else
                outInput.x = 0f;

            // Y axis
            if (Mathf.Abs(input.y) >= deadzone)
                outInput.y = input.y;
            else
                outInput.y = 0f;

            return outInput;
        }

        /// <summary>
        /// Input System callback for WASD movement.
        /// </summary>
        private void OnMove(InputValue input)
        {
            moveInput = input.Get<Vector2>();

            bool wasMoving = isMoving;
            isMoving = moveInput.sqrMagnitude > 0.01f;

            // Handle movement audio
            if (isMoving)
            {
                // Cancel fade if transitioning from stopped to moving
                if (fadeRoutine != null)
                {
                    StopCoroutine(fadeRoutine);
                    fadeRoutine = null;
                }

                movementAudio.volume = 1f;

                if (!movementAudio.isPlaying)
                {
                    movementAudio.Play();
                }

                // Trigger initial footstep pulse when starting to move
                if (!wasMoving && enableFootstepPulses)
                {
                    TriggerFootstepPulse();
                    lastFootstepPulseTime = Time.time;
                }
            }
            else
            {
                // Fade out audio when stopping
                if (movementAudio.isPlaying && fadeRoutine == null)
                {
                    fadeRoutine = StartCoroutine(FadeOutAudio(movementAudio, fadeOutDuration));
                }
            }
        }

        /// <summary>
        /// Input System callback for manual echolocation pulse (E key).
        /// </summary>
        private void OnPulse(InputValue input)
        {
            if (echolocationSystem != null)
            {
                echolocationSystem.TriggerManualPulse();
            }
            else
            {
                Debug.LogWarning("Cannot trigger pulse - EcholocationSystem not found");
            }
        }

        /// <summary>
        /// Triggers a footstep echolocation pulse.
        /// </summary>
        private void TriggerFootstepPulse()
        {
            if (echolocationSystem != null)
            {
                echolocationSystem.TriggerFootstepPulse();
            }
        }

        /// <summary>
        /// Fades out audio over specified duration.
        /// </summary>
        private IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
        {
            float startVolume = audioSource.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            audioSource.volume = 0f;
            audioSource.Stop();
            audioSource.volume = startVolume; // Reset for next play

            fadeRoutine = null;
        }

        /// <summary>
        /// Debug teleport to target position (preserves Y coordinate).
        /// </summary>
        private void DebugTeleport()
        {
            // Preserve current Y position
            Vector3 targetPosition = new Vector3(
                debugTeleportPosition.x,
                transform.position.y, // Keep current Y
                debugTeleportPosition.z
            );

            transform.position = targetPosition;

            // Reset velocity to prevent momentum carrying over
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }

            Debug.Log($"Debug teleported to: {targetPosition}");
        }

        /// <summary>
        /// Returns current movement input.
        /// </summary>
        public Vector2 GetMoveInput()
        {
            return moveInput;
        }

        /// <summary>
        /// Returns true if player is currently moving.
        /// </summary>
        public bool IsMoving()
        {
            return isMoving;
        }
    }
}