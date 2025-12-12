using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BrainNotBraining.Gameplay;
using BrainNotBraining.Core;

/// <summary>
/// Manages echolocation pulse system for Level 3 (Somatosensory Cortex).
/// Handles both automatic footstep pulses and manual player-triggered pulses.
/// Uses raycasting to detect surfaces and trigger visual outline reveals.
/// </summary>
public class EcholocationSystem : MonoBehaviour
{
    [Header("Pulse Configuration")]
    [Tooltip("Radius for footstep-triggered pulses")]
    public float footstepRadius = 8f;

    [Tooltip("Radius for manually-triggered pulses (E key)")]
    public float manualRadius = 15f;

    [Tooltip("Number of rays cast for footstep pulses")]
    public int footstepRayCount = 32;

    [Tooltip("Number of rays cast for manual pulses")]
    public int manualRayCount = 64;

    [Header("Reveal Duration")]
    [Tooltip("How long outlines stay visible after footstep pulse")]
    public float footstepRevealDuration = 1.5f;

    [Tooltip("How long outlines stay visible after manual pulse")]
    public float manualRevealDuration = 3.0f;

    [Tooltip("Fade-out duration for footstep pulse outlines")]
    public float footstepFadeDuration = 0.5f;

    [Tooltip("Fade-out duration for manual pulse outlines")]
    public float manualFadeDuration = 0.8f;

    [Header("Pulse Brightness (Shader Integration)")]
    [Tooltip("Brightness multiplier for footstep pulses (affects edge brightness in shader)")]
    [Range(0.5f, 3f)]
    public float footstepPulseStrength = 1.5f;

    [Tooltip("Brightness multiplier for manual pulses (affects edge brightness in shader)")]
    [Range(0.5f, 3f)]
    public float manualPulseStrength = 2.5f;

    [Header("Audio Configuration")]
    [Tooltip("Audio clip for pulse sound")]
    public AudioClip pulseSound;

    [Tooltip("Volume for footstep pulses (0-1)")]
    [Range(0f, 1f)]
    public float footstepVolume = 0.6f;

    [Tooltip("Volume for manual pulses (0-1)")]
    [Range(0f, 1f)]
    public float manualVolume = 0.85f;

    [Header("Cooldown")]
    [Tooltip("Minimum time between footstep pulses")]
    public float footstepCooldown = 0.3f;

    [Tooltip("Minimum time between manual pulses")]
    public float manualCooldown = 2.0f;

    [Header("Layer Configuration")]
    [Tooltip("Layers that respond to echolocation pulses")]
    public LayerMask echoLayerMask = ~0; // Default: all layers

    // Internal state
    private AudioSource audioSource;
    private float lastFootstepPulseTime = -999f;
    private float lastManualPulseTime = -999f;

    // System references
    private OutlineRevealManager outlineManager;
    private EcholocationPulseManager pulseManager;

    private void Awake()
    {
        // Create dedicated audio source for pulses
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.priority = 128; // Medium priority
    }

    private void Start()
    {
        // Find outline manager in scene (will be created later)
        outlineManager = FindObjectOfType<OutlineRevealManager>();

        if (outlineManager == null)
        {
            Debug.LogWarning("EcholocationSystem: No OutlineRevealManager found in scene. Outline reveals will not work.");
        }

        // Find pulse manager (prefer singleton instance)
        pulseManager = EcholocationPulseManager.Instance;

        if (pulseManager == null)
        {
            Debug.LogWarning("EcholocationSystem: No EcholocationPulseManager found. Visual pulse expansion effects will not work.");
        }
    }

    /// <summary>
    /// Triggers a footstep echolocation pulse from the current position.
    /// </summary>
    public void TriggerFootstepPulse()
    {
        // Check cooldown
        if (Time.time - lastFootstepPulseTime < footstepCooldown)
        {
            return;
        }

        lastFootstepPulseTime = Time.time;

        // Trigger pulse with footstep parameters
        TriggerPulse(footstepRadius, footstepRayCount, footstepRevealDuration,
                    footstepFadeDuration, footstepVolume, footstepPulseStrength, PulseType.Footstep);
    }

    /// <summary>
    /// Triggers a manual echolocation pulse (E key) from the current position.
    /// </summary>
    public void TriggerManualPulse()
    {
        // Check cooldown
        if (Time.time - lastManualPulseTime < manualCooldown)
        {
            return;
        }

        lastManualPulseTime = Time.time;

        // Trigger pulse with manual parameters
        TriggerPulse(manualRadius, manualRayCount, manualRevealDuration,
                    manualFadeDuration, manualVolume, manualPulseStrength, PulseType.Manual);
    }

    /// <summary>
    /// Core pulse logic - casts rays spherically and triggers outline reveals.
    /// </summary>
    private void TriggerPulse(float radius, int rayCount, float revealDuration,
                              float fadeDuration, float volume, float pulseStrength, PulseType pulseType)
    {
        // Play audio
        if (pulseSound != null && audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(pulseSound);
        }

        // Trigger visual pulse expansion effect (shader-based)
        if (pulseManager != null)
        {
            float pulseDuration = revealDuration + fadeDuration;
            pulseManager.TriggerPulse(transform.position, radius, pulseDuration, pulseStrength);
        }

        // Cast rays in Fibonacci sphere distribution
        List<RaycastHit> hits = new List<RaycastHit>();

        for (int i = 0; i < rayCount; i++)
        {
            Vector3 direction = GetFibonacciSpherePoint(i, rayCount);

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, radius, echoLayerMask))
            {
                // Check if object is echolocatable
                if (IsEcholocatable(hit.collider))
                {
                    hits.Add(hit);
                }
            }
        }

        // Trigger outline reveals at hit positions (particle effects)
        if (outlineManager != null)
        {
            foreach (RaycastHit hit in hits)
            {
                outlineManager.RevealOutlineAt(hit.point, hit.normal, revealDuration, fadeDuration);
            }
        }

        // Debug visualization in editor
#if UNITY_EDITOR
            if (pulseType == PulseType.Manual)
            {
                Debug.Log($"Echolocation pulse: {hits.Count} surfaces detected (radius: {radius}m, rays: {rayCount})");
            }
#endif
    }

    /// <summary>
    /// Generates evenly distributed points on a sphere using Fibonacci spiral.
    /// </summary>
    private Vector3 GetFibonacciSpherePoint(int index, int totalPoints)
    {
        float goldenRatio = (1f + Mathf.Sqrt(5f)) / 2f;
        float angleIncrement = Mathf.PI * 2f * goldenRatio;

        float t = (float)index / totalPoints;
        float inclination = Mathf.Acos(1f - 2f * t);
        float azimuth = angleIncrement * index;

        float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
        float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
        float z = Mathf.Cos(inclination);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Checks if a collider should respond to echolocation pulses.
    /// </summary>
    private bool IsEcholocatable(Collider collider)
    {
        // Check for specific tags
        if (collider.CompareTag("EchoSurface") ||
            collider.CompareTag("Interactable") ||
            collider.CompareTag("Wall"))
        {
            return true;
        }

        // Default: accept any non-player, non-trigger collider
        if (collider.CompareTag("Player") || collider.isTrigger)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns true if footstep pulse is off cooldown.
    /// </summary>
    public bool CanFootstepPulse()
    {
        return Time.time - lastFootstepPulseTime >= footstepCooldown;
    }

    /// <summary>
    /// Returns true if manual pulse is off cooldown.
    /// </summary>
    public bool CanManualPulse()
    {
        return Time.time - lastManualPulseTime >= manualCooldown;
    }

    /// <summary>
    /// Returns normalized cooldown remaining for manual pulse (0-1).
    /// Useful for UI feedback.
    /// </summary>
    public float GetManualPulseCooldownNormalized()
    {
        float timeSinceLastPulse = Time.time - lastManualPulseTime;
        if (timeSinceLastPulse >= manualCooldown)
        {
            return 0f;
        }

        return 1f - (timeSinceLastPulse / manualCooldown);
    }

    private enum PulseType
    {
        Footstep,
        Manual
    }

#if UNITY_EDITOR
        // Debug visualization
        private void OnDrawGizmosSelected()
        {
            // Draw footstep pulse radius (green)
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, footstepRadius);

            // Draw manual pulse radius (cyan)
            Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, manualRadius);
        }
#endif
}