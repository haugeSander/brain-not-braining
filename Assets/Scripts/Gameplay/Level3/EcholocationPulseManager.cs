using UnityEngine;
using System.Collections.Generic;

namespace BrainNotBraining.Gameplay
{
    /// <summary>
    /// Manages active echolocation pulses and updates shader properties for visual effects.
    /// Pulses expand outward in a growing sphere, then fade uniformly over time.
    /// Multiple pulses use maximum brightness (no stacking beyond strongest pulse).
    /// Integrates with ProximityEdgeBrightness by adding pulse boost on top of base proximity.
    /// </summary>
    public class EcholocationPulseManager : MonoBehaviour
    {
        [Header("Pulse Configuration")]
        [Tooltip("Maximum number of simultaneous active pulses")]
        public int maxActivePulses = 8;

        [Tooltip("Pulse expansion speed (meters per second)")]
        public float pulseExpansionSpeed = 10f;

        [Tooltip("Minimum brightness threshold (pulses below this are culled)")]
        [Range(0f, 0.1f)]
        public float minBrightness = 0.01f;

        [Header("Debug")]
        [Tooltip("Show debug info in console")]
        public bool debugMode = false;

        [Tooltip("Draw pulse gizmos in editor")]
        public bool drawGizmos = true;

        // Singleton instance
        public static EcholocationPulseManager Instance { get; private set; }

        // Active pulses list
        private List<PulseData> activePulses = new List<PulseData>();

        // Shader property IDs (cached for performance)
        private static readonly int PulseDataID = Shader.PropertyToID("_PulseData");
        private static readonly int PulseBrightnessID = Shader.PropertyToID("_PulseBrightness");
        private static readonly int PulseMaxRadiusID = Shader.PropertyToID("_PulseMaxRadius");

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("EcholocationPulseManager: Multiple instances detected! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // Initialize shader properties to zero
            ClearShaderProperties();

            if (debugMode)
            {
                Debug.Log("EcholocationPulseManager: Initialized successfully");
            }
        }

        private void Update()
        {
            // Update all active pulses
            UpdatePulses();

            // Find strongest pulse and send to shader
            UpdateShaderProperties();
        }

        /// <summary>
        /// Triggers a new echolocation pulse at the specified position.
        /// </summary>
        /// <param name="position">World position of pulse origin</param>
        /// <param name="maxRadius">Maximum radius the pulse will expand to</param>
        /// <param name="duration">Total duration including fade time (seconds)</param>
        /// <param name="strength">Brightness multiplier (1.0 = normal, higher = brighter)</param>
        public void TriggerPulse(Vector3 position, float maxRadius, float duration, float strength = 1.0f)
        {
            // Check if we're at capacity
            if (activePulses.Count >= maxActivePulses)
            {
                // Remove oldest pulse to make room
                activePulses.RemoveAt(0);

                if (debugMode)
                {
                    Debug.LogWarning($"EcholocationPulseManager: Max pulses reached ({maxActivePulses}), removing oldest");
                }
            }

            // Create new pulse data
            PulseData newPulse = new PulseData
            {
                startTime = Time.time,
                startPosition = position,
                maxRadius = maxRadius,
                duration = duration,
                strength = strength,
                currentRadius = 0f,
                currentBrightness = strength
            };

            activePulses.Add(newPulse);

            if (debugMode)
            {
                Debug.Log($"EcholocationPulseManager: Triggered pulse at {position}, maxRadius={maxRadius:F1}m, duration={duration:F1}s, strength={strength:F2}");
            }
        }

        /// <summary>
        /// Updates all active pulses (expansion and fade).
        /// </summary>
        private void UpdatePulses()
        {
            for (int i = activePulses.Count - 1; i >= 0; i--)
            {
                PulseData pulse = activePulses[i];
                float elapsed = Time.time - pulse.startTime;

                // Check if pulse has expired
                if (elapsed >= pulse.duration || pulse.currentBrightness <= minBrightness)
                {
                    activePulses.RemoveAt(i);

                    if (debugMode)
                    {
                        Debug.Log($"EcholocationPulseManager: Pulse expired (elapsed={elapsed:F2}s, brightness={pulse.currentBrightness:F3})");
                    }

                    continue;
                }

                // Calculate expansion (linear)
                float expansionProgress = Mathf.Clamp01(elapsed / (pulse.duration * 0.5f)); // Expand in first half of duration
                pulse.currentRadius = expansionProgress * pulse.maxRadius;

                // Calculate fade (starts after expansion completes)
                float fadeStartTime = pulse.duration * 0.5f;
                if (elapsed > fadeStartTime)
                {
                    float fadeProgress = (elapsed - fadeStartTime) / (pulse.duration - fadeStartTime);
                    pulse.currentBrightness = Mathf.Lerp(pulse.strength, 0f, fadeProgress);
                }
                else
                {
                    pulse.currentBrightness = pulse.strength;
                }

                // Update pulse in list
                activePulses[i] = pulse;
            }
        }

        /// <summary>
        /// Finds the strongest pulse and updates shader global properties.
        /// Uses maximum brightness strategy (brightest pulse wins at each point).
        /// </summary>
        private void UpdateShaderProperties()
        {
            if (activePulses.Count == 0)
            {
                // No active pulses - clear shader properties
                ClearShaderProperties();
                return;
            }

            // Find strongest pulse (highest brightness)
            PulseData strongestPulse = activePulses[0];
            for (int i = 1; i < activePulses.Count; i++)
            {
                if (activePulses[i].currentBrightness > strongestPulse.currentBrightness)
                {
                    strongestPulse = activePulses[i];
                }
            }

            // Set shader globals for strongest pulse
            Vector4 pulseData = new Vector4(
                strongestPulse.startPosition.x,
                strongestPulse.startPosition.y,
                strongestPulse.startPosition.z,
                strongestPulse.currentRadius
            );

            Shader.SetGlobalVector(PulseDataID, pulseData);
            Shader.SetGlobalFloat(PulseBrightnessID, strongestPulse.currentBrightness);
            Shader.SetGlobalFloat(PulseMaxRadiusID, strongestPulse.maxRadius);

            // Debug logging to verify shader updates
            if (debugMode && activePulses.Count > 0)
            {
                Debug.Log($"[PULSE SHADER UPDATE] Active: {activePulses.Count} | Position: ({pulseData.x:F1}, {pulseData.y:F1}, {pulseData.z:F1}) | Radius: {strongestPulse.currentRadius:F2}/{strongestPulse.maxRadius:F1}m | Brightness: {strongestPulse.currentBrightness:F3}");
            }
        }

        /// <summary>
        /// Clears shader properties (sets pulse brightness to zero).
        /// </summary>
        private void ClearShaderProperties()
        {
            Shader.SetGlobalVector(PulseDataID, Vector4.zero);
            Shader.SetGlobalFloat(PulseBrightnessID, 0f);
            Shader.SetGlobalFloat(PulseMaxRadiusID, 0f);
        }

        /// <summary>
        /// Returns the number of currently active pulses.
        /// </summary>
        public int GetActivePulseCount()
        {
            return activePulses.Count;
        }

        /// <summary>
        /// Clears all active pulses immediately.
        /// </summary>
        public void ClearAllPulses()
        {
            activePulses.Clear();
            ClearShaderProperties();

            if (debugMode)
            {
                Debug.Log("EcholocationPulseManager: All pulses cleared");
            }
        }

        private void OnDisable()
        {
            // Clean up shader properties when disabled
            ClearShaderProperties();
        }

        private void OnDestroy()
        {
            // Clear singleton reference
            if (Instance == this)
            {
                Instance = null;
            }

            // Clean up shader properties
            ClearShaderProperties();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!drawGizmos || activePulses == null)
            {
                return;
            }

            // Draw each active pulse as a wire sphere
            foreach (PulseData pulse in activePulses)
            {
                // Color based on brightness (fade from yellow to red)
                float normalizedBrightness = pulse.currentBrightness / pulse.strength;
                Color pulseColor = Color.Lerp(Color.red, Color.yellow, normalizedBrightness);
                pulseColor.a = 0.5f;

                Gizmos.color = pulseColor;
                Gizmos.DrawWireSphere(pulse.startPosition, pulse.currentRadius);

                // Draw max radius as faint outline
                Gizmos.color = new Color(pulseColor.r, pulseColor.g, pulseColor.b, 0.1f);
                Gizmos.DrawWireSphere(pulse.startPosition, pulse.maxRadius);
            }
        }
#endif

        /// <summary>
        /// Data structure for a single pulse instance.
        /// </summary>
        private struct PulseData
        {
            public float startTime;
            public Vector3 startPosition;
            public float maxRadius;
            public float duration;
            public float strength;
            public float currentRadius;
            public float currentBrightness;
        }
    }
}
