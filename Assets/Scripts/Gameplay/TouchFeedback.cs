using UnityEngine;
using System.Collections;
using BrainNotBraining.Core;
using BrainNotBraining.Gameplay;

/// <summary>
/// Provides touch-based interaction feedback for Level 3.
/// Differentiates between light/heavy objects and switches.
/// Triggers audio, haptic, and visual (pulse) feedback on interaction.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TouchFeedback : MonoBehaviour
{
    [Header("Interaction Type")]
    [Tooltip("Type of interactable object")]
    public InteractionType interactionType = InteractionType.LightBlock;

    [Header("Audio Configuration")]
    [Tooltip("Audio clip for light block interaction")]
    public AudioClip lightBlockSound;

    [Tooltip("Audio clip for heavy block interaction")]
    public AudioClip heavyBlockSound;

    [Tooltip("Audio clip for switch press")]
    public AudioClip switchClickSound;

    [Tooltip("Volume for interaction sounds (0-1)")]
    [Range(0f, 1f)]
    public float audioVolume = 0.8f;

    [Header("Pitch Configuration")]
    [Tooltip("Pitch for light block sound (default: 1.2 - high-pitched)")]
    public float lightBlockPitch = 1.2f;

    [Tooltip("Pitch for heavy block sound (default: 0.7 - low-pitched)")]
    public float heavyBlockPitch = 0.7f;

    [Tooltip("Pitch for switch sound (default: 1.0 - normal)")]
    public float switchPitch = 1.0f;

    [Header("Haptic Configuration")]
    [Tooltip("Duration of haptic pulse for light blocks (seconds)")]
    public float lightBlockHapticDuration = 0.1f;

    [Tooltip("Duration of haptic pulse for heavy blocks (seconds)")]
    public float heavyBlockHapticDuration = 0.4f;

    [Tooltip("Duration of haptic pulse for switches (seconds)")]
    public float switchHapticDuration = 0.15f;

    [Header("Echolocation Pulse")]
    [Tooltip("Trigger local echolocation pulse on interaction")]
    public bool triggerPulseOnInteraction = true;

    [Tooltip("Radius of echolocation pulse triggered by interaction")]
    public float interactionPulseRadius = 5f;

    [Header("Switch Configuration")]
    [Tooltip("Is this switch currently pressed?")]
    public bool isSwitchPressed = false;

    [Tooltip("Can switch be pressed multiple times?")]
    public bool allowMultiplePresses = false;

    // Internal state
    private AudioSource audioSource;
    private Rigidbody rb;
    private float lastCollisionTime = -999f;
    private float collisionCooldown = 0.2f; // Prevent spam
    private EcholocationSystem playerEcholocation;

    // Mass thresholds for automatic type detection
    private const float LIGHT_BLOCK_MASS_THRESHOLD = 5f;

    private void Awake()
    {
        // Create audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0.5f; // Semi-spatial
        audioSource.priority = 64; // Higher priority for interaction sounds

        // Get rigidbody if exists (for blocks)
        rb = GetComponent<Rigidbody>();

        // Auto-detect interaction type based on mass if Rigidbody exists
        if (rb != null && interactionType == InteractionType.LightBlock)
        {
            if (rb.mass > LIGHT_BLOCK_MASS_THRESHOLD)
            {
                interactionType = InteractionType.HeavyBlock;
            }
        }
    }

    private void Start()
    {
        // Find player's echolocation system
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerEcholocation = player.GetComponent<EcholocationSystem>();
        }
    }

    /// <summary>
    /// Handles collision with player or other objects.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Check cooldown
        if (Time.time - lastCollisionTime < collisionCooldown)
        {
            return;
        }

        // Only trigger on player collision
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        lastCollisionTime = Time.time;

        // Trigger feedback based on interaction type
        switch (interactionType)
        {
            case InteractionType.LightBlock:
                TriggerLightBlockFeedback();
                break;

            case InteractionType.HeavyBlock:
                TriggerHeavyBlockFeedback();
                break;
        }

        // Trigger echolocation pulse if enabled
        if (triggerPulseOnInteraction)
        {
            TriggerInteractionPulse();
        }
    }

    /// <summary>
    /// Handles trigger collision for switches.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (interactionType != InteractionType.Switch)
        {
            return;
        }

        // Only trigger on player
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Check if switch can be pressed
        if (isSwitchPressed && !allowMultiplePresses)
        {
            return;
        }

        PressSwitch();
    }

    /// <summary>
    /// Triggers feedback for light block interaction.
    /// </summary>
    private void TriggerLightBlockFeedback()
    {
        // Play audio
        if (lightBlockSound != null)
        {
            audioSource.pitch = lightBlockPitch;
            audioSource.volume = audioVolume;
            audioSource.PlayOneShot(lightBlockSound);
        }

        // Trigger haptic
        TriggerHaptic(lightBlockHapticDuration);

        Debug.Log("Light block touched");
    }

    /// <summary>
    /// Triggers feedback for heavy block interaction.
    /// </summary>
    private void TriggerHeavyBlockFeedback()
    {
        // Play audio
        if (heavyBlockSound != null)
        {
            audioSource.pitch = heavyBlockPitch;
            audioSource.volume = audioVolume;
            audioSource.PlayOneShot(heavyBlockSound);
        }

        // Trigger haptic
        TriggerHaptic(heavyBlockHapticDuration);

        Debug.Log("Heavy block touched");
    }

    /// <summary>
    /// Handles switch press interaction.
    /// </summary>
    private void PressSwitch()
    {
        isSwitchPressed = true;

        // Play click sound
        if (switchClickSound != null)
        {
            audioSource.pitch = switchPitch;
            audioSource.volume = audioVolume;
            audioSource.PlayOneShot(switchClickSound);
        }

        // Trigger haptic
        TriggerHaptic(switchHapticDuration);

        // Trigger echolocation pulse
        if (triggerPulseOnInteraction)
        {
            TriggerInteractionPulse();
        }

        // Visual feedback (could animate switch here)
        OnSwitchPressed();

        Debug.Log("Switch pressed");
    }

    /// <summary>
    /// Triggers local echolocation pulse from interaction position.
    /// </summary>
    private void TriggerInteractionPulse()
    {
        if (playerEcholocation != null)
        {
            // Create temporary echolocation system at interaction point
            GameObject tempPulse = new GameObject("InteractionPulse");
            tempPulse.transform.position = transform.position;

            EcholocationSystem tempEcho = tempPulse.AddComponent<EcholocationSystem>();
            tempEcho.manualRadius = interactionPulseRadius;
            tempEcho.manualRayCount = 32;
            tempEcho.manualRevealDuration = 2f;
            tempEcho.pulseSound = playerEcholocation.pulseSound;

            // Trigger pulse
            tempEcho.TriggerManualPulse();

            // Destroy after pulse completes
            Destroy(tempPulse, 0.5f);
        }
    }

    /// <summary>
    /// Triggers haptic feedback (placeholder for gamepad support).
    /// </summary>
    private void TriggerHaptic(float duration)
    {
        // TODO: Implement with Input System when gamepad support is added
        // Example:
        // var gamepad = Gamepad.current;
        // if (gamepad != null)
        // {
        //     float intensity = interactionType == InteractionType.HeavyBlock ? 0.8f : 0.3f;
        //     gamepad.SetMotorSpeeds(intensity, intensity);
        //     StartCoroutine(StopHapticsAfterDelay(duration));
        // }

        // For now, just log
        Debug.Log($"Haptic feedback: {duration}s");
    }

    /// <summary>
    /// Called when switch is pressed - override for custom behavior.
    /// </summary>
    protected virtual void OnSwitchPressed()
    {
        // Override in derived classes for switch-specific logic
        // Could trigger door opening, activate circuit, etc.
    }

    /// <summary>
    /// Resets switch to unpressed state.
    /// </summary>
    public void ResetSwitch()
    {
        isSwitchPressed = false;
    }

    /// <summary>
    /// Returns true if this is a switch and it's pressed.
    /// </summary>
    public bool IsSwitchPressed()
    {
        return interactionType == InteractionType.Switch && isSwitchPressed;
    }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Visualize interaction pulse radius
            if (triggerPulseOnInteraction)
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
                Gizmos.DrawWireSphere(transform.position, interactionPulseRadius);
            }

            // Label interaction type
            Color labelColor = interactionType switch
            {
                InteractionType.LightBlock => Color.green,
                InteractionType.HeavyBlock => Color.red,
                InteractionType.Switch => Color.cyan,
                _ => Color.white
            };

            UnityEditor.Handles.color = labelColor;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, interactionType.ToString());
        }
#endif
}

/// <summary>
/// Types of touch interactions.
/// </summary>
public enum InteractionType
{
    LightBlock,
    HeavyBlock,
    Switch
}
