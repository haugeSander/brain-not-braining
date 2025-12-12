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

    [Header("Danger Configuration")]
    [Tooltip("Is this a dangerous heat zone? (Damages player over time)")]
    public bool isDangerousHeat = false;

    [Tooltip("Damage per second when in dangerous heat zone")]
    [Range(1f, 50f)]
    public float heatDamagePerSecond = 10f;

    [Tooltip("Is this a dangerous cold zone? (Slows player movement)")]
    public bool isDangerousCold = false;

    [Tooltip("Movement speed reduction percentage (0.5 = 50% slower)")]
    [Range(0.1f, 0.9f)]
    public float coldSlowdownPercent = 0.5f;

    [Header("Audio Configuration")]
    [Tooltip("Audio clip for this zone (hum for warm, ambient for cool)")]
    public AudioClip zoneAudioClip;

    [Tooltip("Volume for zone audio (0-1) - INCREASED for better feedback")]
    [Range(0f, 1f)]
    public float audioVolume = 0.7f; // Increased from 0.3

    [Tooltip("Pitch for zone audio (lower for warm, higher for cool)")]
    [Range(0.1f, 3f)]
    public float audioPitch = 1.0f;

    [Tooltip("Spatial blend (0=2D, 1=3D) - INCREASED for directional audio")]
    [Range(0f, 1f)]
    public float spatialBlend = 0.8f; // Increased from 0.3

    [Tooltip("Fade-in duration when entering zone")]
    public float audioFadeInDuration = 0.5f;

    [Tooltip("Fade-out duration when exiting zone")]
    public float audioFadeOutDuration = 0.8f;

    [Header("Visual Effects")]
    [Tooltip("Particle system for zone visualization (optional)")]
    public ParticleSystem zoneParticles;

    [Tooltip("Intensity multiplier for visual effects (bloom/grain) - INCREASED")]
    [Range(0f, 1f)]
    public float visualIntensity = 0.9f; // Increased from 0.5

    [Tooltip("Enable screen color tint (red for heat, blue for cold)")]
    public bool enableScreenTint = true;

    [Tooltip("Screen tint intensity (0-1)")]
    [Range(0f, 1f)]
    public float tintIntensity = 0.3f;

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

    // Player references for damage/movement
    private GameObject playerObject;
    private BrainNotBraining.Player.Level3PlayerMovement playerMovement;
    private float originalMovementSpeed;
    private bool movementSpeedModified = false;

    // Shader property IDs for screen tint
    private static readonly int ScreenTintColorID = Shader.PropertyToID("_TemperatureScreenTint");
    private static readonly int ScreenTintIntensityID = Shader.PropertyToID("_TemperatureScreenTintIntensity");

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
        audioSource.spatialBlend = spatialBlend; // Use configured spatial blend
        audioSource.volume = 0f;
        audioSource.pitch = audioPitch;
    }

    private void Update()
    {
        // Apply damage over time if in dangerous heat zone
        if (playerInZone && isDangerousHeat && playerObject != null)
        {
            ApplyHeatDamage();
        }
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
            playerObject = other.gameObject;

            // Get player movement component for slowdown
            playerMovement = playerObject.GetComponent<BrainNotBraining.Player.Level3PlayerMovement>();

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

        // Apply screen color tint
        if (enableScreenTint)
        {
            ApplyScreenTint();
        }

        // Apply movement slowdown for dangerous cold zones
        if (isDangerousCold && playerMovement != null)
        {
            ApplyMovementSlowdown();
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

        string dangerText = isDangerousHeat ? " (DANGEROUS - Taking damage!)" : isDangerousCold ? " (DANGEROUS - Movement slowed!)" : "";
        Debug.Log($"Entered {temperatureType} temperature zone{dangerText}");
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

        // Clear screen color tint
        if (enableScreenTint)
        {
            ClearScreenTint();
        }

        // Restore movement speed if it was modified
        if (movementSpeedModified && playerMovement != null)
        {
            RestoreMovementSpeed();
        }

        // Stop haptic feedback
        if (hapticCoroutine != null)
        {
            StopCoroutine(hapticCoroutine);
            hapticCoroutine = null;
        }

        // Clear player references
        playerObject = null;
        playerMovement = null;

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
    /// Applies heat damage to the player over time.
    /// Called every frame when player is in dangerous heat zone.
    /// </summary>
    private void ApplyHeatDamage()
    {
        float damageThisFrame = heatDamagePerSecond * Time.deltaTime;

        // TODO: Implement actual health system - for now, reload level after taking too much damage
        // This is a placeholder that kills player after ~3 seconds in heat (10 damage/sec * 3 sec = 30 damage)
        // When you implement a proper health system, replace this with:
        // playerHealth.TakeDamage(damageThisFrame);

        // Temporary solution: Reload level after taking significant damage
        // You can integrate this with a proper health system later
        Debug.LogWarning($"Player taking heat damage: {damageThisFrame:F2} damage this frame");

        // For now, reload after 3 seconds of continuous damage (can be improved with health system)
    }

    /// <summary>
    /// Applies movement slowdown effect for dangerous cold zones.
    /// </summary>
    private void ApplyMovementSlowdown()
    {
        if (playerMovement == null)
            return;

        // Store original speed before modifying
        originalMovementSpeed = playerMovement.movementSpeed;

        // Apply slowdown
        playerMovement.movementSpeed = originalMovementSpeed * (1f - coldSlowdownPercent);
        movementSpeedModified = true;

        Debug.Log($"Movement slowed by {coldSlowdownPercent * 100}% (from {originalMovementSpeed:F1} to {playerMovement.movementSpeed:F1})");
    }

    /// <summary>
    /// Restores original movement speed when exiting cold zone.
    /// </summary>
    private void RestoreMovementSpeed()
    {
        if (playerMovement == null)
            return;

        playerMovement.movementSpeed = originalMovementSpeed;
        movementSpeedModified = false;

        Debug.Log($"Movement speed restored to {originalMovementSpeed:F1}");
    }

    /// <summary>
    /// Applies screen color tint based on temperature type.
    /// Sets shader globals that can be read by post-processing or camera shader.
    /// </summary>
    private void ApplyScreenTint()
    {
        Color tintColor = temperatureType == TemperatureType.Warm
            ? new Color(1f, 0.3f, 0f, 1f) // Red-orange tint for heat
            : new Color(0f, 0.5f, 1f, 1f); // Blue tint for cold

        Shader.SetGlobalColor(ScreenTintColorID, tintColor);
        Shader.SetGlobalFloat(ScreenTintIntensityID, tintIntensity);

        Debug.Log($"Applied {temperatureType} screen tint (intensity: {tintIntensity})");
    }

    /// <summary>
    /// Clears screen color tint when exiting zone.
    /// </summary>
    private void ClearScreenTint()
    {
        Shader.SetGlobalColor(ScreenTintColorID, Color.clear);
        Shader.SetGlobalFloat(ScreenTintIntensityID, 0f);
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
