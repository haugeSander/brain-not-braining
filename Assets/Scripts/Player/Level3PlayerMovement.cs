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

        private void FixedUpdate()
        {
            // Apply movement force
            Vector2 filteredInput = ApplyDeadzone(moveInput, 0.2f);

            // Compute direction (or zero)
            Vector3 moveDirection = Vector3.zero;

            if (filteredInput.sqrMagnitude > 0.01f)
            {
                if (useCameraRelativeMovement && mainCamera != null)
                {
                    Vector3 cameraForward = mainCamera.transform.forward;
                    cameraForward.y = 0f;
                    cameraForward.Normalize();

                    Vector3 cameraRight = mainCamera.transform.right;
                    cameraRight.y = 0f;
                    cameraRight.Normalize();

                    moveDirection = (cameraForward * filteredInput.y + cameraRight * filteredInput.x).normalized;
                }
                else
                {
                    moveDirection = new Vector3(filteredInput.x, 0f, filteredInput.y).normalized;
                }
            }

            // Apply velocity
            Vector3 newVelocity;

            if (moveDirection == Vector3.zero)
            {
                // No input → STOP fully except gravity
                newVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
            else
            {
                // Controlled movement
                newVelocity = moveDirection * movementSpeed;
                newVelocity.y = rb.linearVelocity.y;
            }

            rb.linearVelocity = newVelocity;
        }

        private Vector2 ApplyDeadzone(Vector2 input, float deadzone)
        {
            if (input.sqrMagnitude < deadzone * deadzone)
                return Vector2.zero;

            return input;
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