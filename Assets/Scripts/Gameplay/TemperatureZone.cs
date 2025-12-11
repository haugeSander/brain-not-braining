using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using BrainNotBraining.Core;
using BrainNotBraining.Gameplay;

/// <summary>
/// Represents a warm or cool temperature zone in Level 3.
/// Provides audio, visual, and (optionally) haptic feedback to differentiate zones.
/// Uses monochrome effects only (no color).
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class TemperatureZone : MonoBehaviour
{
    [Header("Zone Configuration")]
    [Tooltip("Type of temperature zone")]
    public TemperatureType temperatureType = TemperatureType.Warm;

    [Header("Audio Configuration")]
    [Tooltip("Audio clip for this zone (hum for warm, ambient for cool)")]
    public AudioClip zoneAudioClip;

    [Tooltip("Volume for zone audio (0-1)")]
    [Range(0f, 1f)]
    public float audioVolume = 0.3f;

    [Tooltip("Pitch for zone audio (lower for warm, higher for cool)")]
    [Range(0.1f, 3f)]
    public float audioPitch = 1.0f;

    [Tooltip("Fade-in duration when entering zone")]
    public float audioFadeInDuration = 0.5f;

    [Tooltip("Fade-out duration when exiting zone")]
    public float audioFadeOutDuration = 0.8f;

    [Header("Visual Effects")]
    [Tooltip("Particle system for zone visualization (optional)")]
    public ParticleSystem zoneParticles;

    [Tooltip("Intensity multiplier for visual effects (bloom/grain)")]
    [Range(0f, 1f)]
    public float visualIntensity = 0.5f;

    [Header("Haptic Feedback (Optional)")]
    [Tooltip("Enable controller haptic feedback")]
    public bool enableHaptics = false;

    [Tooltip("Haptic frequency for warm zones (slow rumble)")]
    public float warmHapticFrequency = 2f; // Hz

    [Tooltip("Haptic frequency for cool zones (fast pulses)")]
    public float coolHapticFrequency = 10f; // Hz

    // Internal state
    private AudioSource audioSource;
    private bool playerInZone = false;
    private Coroutine audioFadeCoroutine;
    private Coroutine hapticCoroutine;
    private Volume postProcessVolume;

    private void Awake()
    {
        // Ensure BoxCollider is trigger
        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = true;

        // Create audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = zoneAudioClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0.3f; // Slightly spatial
        audioSource.volume = 0f;
        audioSource.pitch = audioPitch;
    }

    private void Start()
    {
        // Set default pitch based on temperature type if not customized
        if (audioPitch == 1.0f)
        {
            audioPitch = temperatureType == TemperatureType.Warm ? 0.7f : 1.8f;
            audioSource.pitch = audioPitch;
        }

        // Start particles if assigned
        if (zoneParticles != null)
        {
            zoneParticles.Play();
        }

        // Try to find post-processing volume (for bloom/grain effects)
        postProcessVolume = GetComponent<Volume>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerInZone)
        {
            playerInZone = true;
            ActivateZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerInZone)
        {
            playerInZone = false;
            DeactivateZone();
        }
    }

    /// <summary>
    /// Activates zone effects when player enters.
    /// </summary>
    private void ActivateZone()
    {
        // Fade in audio
        if (audioFadeCoroutine != null)
        {
            StopCoroutine(audioFadeCoroutine);
        }
        audioFadeCoroutine = StartCoroutine(FadeAudio(0f, audioVolume, audioFadeInDuration));

        // Start audio if not playing
        if (!audioSource.isPlaying && zoneAudioClip != null)
        {
            audioSource.Play();
        }

        // Activate visual effects
        if (postProcessVolume != null)
        {
            postProcessVolume.weight = visualIntensity;
        }

        // Start haptic feedback
        if (enableHaptics)
        {
            if (hapticCoroutine != null)
            {
                StopCoroutine(hapticCoroutine);
            }
            hapticCoroutine = StartCoroutine(HapticFeedbackLoop());
        }

        // Notify AudioManager (if needed for global temperature audio management)
        // Core.AudioManager.Instance?.OnTemperatureZoneEnter(temperatureType);

        Debug.Log($"Entered {temperatureType} temperature zone");
    }

    /// <summary>
    /// Deactivates zone effects when player exits.
    /// </summary>
    private void DeactivateZone()
    {
        // Fade out audio
        if (audioFadeCoroutine != null)
        {
            StopCoroutine(audioFadeCoroutine);
        }
        audioFadeCoroutine = StartCoroutine(FadeAudio(audioSource.volume, 0f, audioFadeOutDuration));

        // Deactivate visual effects
        if (postProcessVolume != null)
        {
            postProcessVolume.weight = 0f;
        }

        // Stop haptic feedback
        if (hapticCoroutine != null)
        {
            StopCoroutine(hapticCoroutine);
            hapticCoroutine = null;
        }

        // Notify AudioManager
        // Core.AudioManager.Instance?.OnTemperatureZoneExit(temperatureType);

        Debug.Log($"Exited {temperatureType} temperature zone");
    }

    /// <summary>
    /// Fades audio volume over time.
    /// </summary>
    private IEnumerator FadeAudio(float startVolume, float targetVolume, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        audioSource.volume = targetVolume;

        // Stop audio if fully faded out
        if (targetVolume == 0f && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Generates haptic feedback pulses based on temperature type.
    /// Note: Unity's Input System haptics require controller support.
    /// </summary>
    private IEnumerator HapticFeedbackLoop()
    {
        float frequency = temperatureType == TemperatureType.Warm ? warmHapticFrequency : coolHapticFrequency;
        float interval = 1f / frequency;
        float pulseDuration = temperatureType == TemperatureType.Warm ? 0.2f : 0.05f;

        while (playerInZone)
        {
            // Trigger haptic pulse
            TriggerHapticPulse(pulseDuration);

            yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>
    /// Triggers a haptic pulse on the active controller.
    /// Placeholder - requires Input System gamepad support.
    /// </summary>
    private void TriggerHapticPulse(float duration)
    {
        // TODO: Implement with Input System when gamepad support is added
        // Example:
        // var gamepad = Gamepad.current;
        // if (gamepad != null)
        // {
        //     float intensity = temperatureType == TemperatureType.Warm ? 0.3f : 0.1f;
        //     gamepad.SetMotorSpeeds(intensity, intensity);
        //     StartCoroutine(StopHapticsAfterDelay(duration));
        // }
    }

    /// <summary>
    /// Returns true if player is currently in this zone.
    /// </summary>
    public bool IsPlayerInZone()
    {
        return playerInZone;
    }

    /// <summary>
    /// Gets the temperature type of this zone.
    /// </summary>
    public TemperatureType GetTemperatureType()
    {
        return temperatureType;
    }

#if UNITY_EDITOR
        // Debug visualization
        private void OnDrawGizmos()
        {
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                // Draw zone bounds with color based on type
                Gizmos.color = temperatureType == TemperatureType.Warm
                    ? new Color(1f, 0.5f, 0f, 0.3f) // Orange for warm
                    : new Color(0f, 0.5f, 1f, 0.3f); // Blue for cool

                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(col.center, col.size);
                Gizmos.DrawWireCube(col.center, col.size);
            }

            // Label
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position, $"{temperatureType} Zone");
#endif
        }
#endif
}

/// <summary>
/// Temperature types for zones.
/// </summary>
public enum TemperatureType
{
    Warm,
    Cool
}
