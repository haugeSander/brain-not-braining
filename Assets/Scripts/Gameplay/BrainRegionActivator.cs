using UnityEngine;
using BrainNotBraining.Core;
using System.Collections;
using System.Collections.Generic;

namespace BrainNotBraining.Gameplay
{
    /// <summary>
    /// Manages the dramatic 3D mouse brain visualization scene.
    /// Lights up brain regions based on progression, with particle effects.
    /// </summary>
    public class BrainRegionActivator : MonoBehaviour
    {
        [Header("Brain Model")]
        [SerializeField] private GameObject brainModel; // Reference to the mouse_brain.obj
        [SerializeField] private Material brainMaterial; // Semi-transparent material
        [SerializeField] private float brainTransparency = 0.3f; // 0 = invisible, 1 = opaque

        [Header("Camera & Rotation")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float rotationSpeed = 20f; // Slow rotation for drama
        [SerializeField] private bool autoRotate = true;

        [Header("Region Lights")]
        [SerializeField] private Light brainstemLight;
        [SerializeField] private Light motorCortexLight;
        [SerializeField] private Light somatosensoryLight;
        [SerializeField] private Light visualCortexLight;
        [SerializeField] private Light prefrontalLight;

        [Header("Light Settings")]
        [SerializeField] private Color activeLightColor = new Color(0f, 1f, 1f, 1f); // Cyan
        [SerializeField] private float lightIntensity = 2f;
        [SerializeField] private float lightRange = 5f;
        [SerializeField] private float lightActivationDuration = 2f; // Fade in time

        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem brainstemParticles;
        [SerializeField] private ParticleSystem motorCortexParticles;
        [SerializeField] private ParticleSystem somatosensoryParticles;
        [SerializeField] private ParticleSystem visualCortexParticles;
        [SerializeField] private ParticleSystem prefrontalParticles;

        [Header("Audio")]
        [SerializeField] private AudioSource dramaticMusicSource;
        [SerializeField] private AudioClip regionUnlockSound;

        [Header("Timing")]
        [SerializeField] private float initialDelay = 0.5f; // Brief pause before brain appears
        [SerializeField] private float displayDuration = 5f; // How long to show brain
        [SerializeField] private float fadeOutDuration = 1f;
        [SerializeField] private string nextSceneName = "MainMenu"; // Scene to load after

        private BrainRegion currentlyActivatingRegion = BrainRegion.None;
        private Dictionary<BrainRegion, Light> regionLights = new Dictionary<BrainRegion, Light>();
        private Dictionary<BrainRegion, ParticleSystem> regionParticles = new Dictionary<BrainRegion, ParticleSystem>();
        private ScreenFlash screenFlash;

        private void Awake()
        {
            // Build lookup dictionaries
            regionLights[BrainRegion.Brainstem] = brainstemLight;
            regionLights[BrainRegion.MotorCortex] = motorCortexLight;
            regionLights[BrainRegion.Somatosensory] = somatosensoryLight;
            regionLights[BrainRegion.VisualCortex] = visualCortexLight;
            regionLights[BrainRegion.Prefrontal] = prefrontalLight;

            regionParticles[BrainRegion.Brainstem] = brainstemParticles;
            regionParticles[BrainRegion.MotorCortex] = motorCortexParticles;
            regionParticles[BrainRegion.Somatosensory] = somatosensoryParticles;
            regionParticles[BrainRegion.VisualCortex] = visualCortexParticles;
            regionParticles[BrainRegion.Prefrontal] = prefrontalParticles;

            // Find ScreenFlash component (should be in scene)
            screenFlash = FindObjectOfType<ScreenFlash>();

            // Set brain material transparency
            if (brainMaterial != null)
            {
                SetBrainTransparency(brainTransparency);
            }

            // Start all lights off
            foreach (var light in regionLights.Values)
            {
                if (light != null)
                {
                    light.intensity = 0f;
                    light.color = activeLightColor;
                    light.range = lightRange;
                }
            }

            // Stop all particle systems
            foreach (var particles in regionParticles.Values)
            {
                if (particles != null)
                {
                    particles.Stop();
                }
            }
        }

        private void Start()
        {
            // Start the activation sequence
            StartCoroutine(ActivationSequence());
        }

        private void Update()
        {
            // Auto-rotate brain for dramatic effect
            if (autoRotate && cameraTransform != null && brainModel != null)
            {
                // Rotate the camera around the brain
                cameraTransform.RotateAround(brainModel.transform.position,
                                            Vector3.up,
                                            rotationSpeed * Time.deltaTime);

                // Keep the camera pointed at the brain
                cameraTransform.LookAt(brainModel.transform);
            }
        }

        private IEnumerator FlickerLight(Light light, float duration, float targetIntensity, float flickerAmount)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                // Smooth fade-in
                float baseIntensity = Mathf.Lerp(0f, targetIntensity, elapsed / duration);

                // Random flicker (tiny, jittery offset)
                float flicker = Random.Range(-flickerAmount, flickerAmount);

                light.intensity = Mathf.Clamp(baseIntensity + flicker, 0f, targetIntensity);

                yield return null;
            }

            light.intensity = targetIntensity;
        }


        /// <summary>
        /// Main sequence: fade in, light up regions, display, fade out, transition
        /// </summary>
        private IEnumerator ActivationSequence()
        {
            // Brief delay
            yield return new WaitForSeconds(initialDelay);

            // Determine which region was just unlocked
            BrainRegion newlyUnlockedRegion = DetermineNewlyUnlockedRegion();
            currentlyActivatingRegion = newlyUnlockedRegion;

            Debug.Log($"BrainRegionActivator: Activating region {newlyUnlockedRegion}");

            // Play dramatic music
            if (regionUnlockSound != null)
            {
                dramaticMusicSource.clip = regionUnlockSound;
                dramaticMusicSource.Play();
            }

            // Light up previously unlocked regions instantly (no particles)
            yield return StartCoroutine(LightUpPreviousRegions(newlyUnlockedRegion));

            // Light up the newly unlocked region with particles and drama
            yield return StartCoroutine(LightUpRegion(newlyUnlockedRegion, true));


            // Save the newly unlocked region to progression
            if (ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.UnlockRegion(newlyUnlockedRegion);
            }

            // Hold display for viewing
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(FadeOutAndTransition());
        }

        /// <summary>
        /// Determine which region was just unlocked based on current level
        /// </summary>
        private BrainRegion DetermineNewlyUnlockedRegion()
        {
            // Check GameManager or ProgressionManager for the current level context
            // For now, we'll use a simple approach: check what was just completed

            // If ProgressionManager has a "pending" or "next to unlock" state, use that
            // Otherwise, default to Brainstem for Level 0

            if (GameManager.Instance != null)
            {
                // You can extend this logic based on scene name or level index
                string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                // For now, assume this scene is only loaded after completing a level
                // and we determine the region from the most recent unlock attempt
            }

            // Simple fallback: check what's NOT yet unlocked
            if (ProgressionManager.Instance != null)
            {
                var unlocked = ProgressionManager.Instance.GetUnlockedRegions();

                if (!unlocked.Contains(BrainRegion.Brainstem))
                    return BrainRegion.Brainstem;
                if (!unlocked.Contains(BrainRegion.MotorCortex))
                    return BrainRegion.MotorCortex;
                if (!unlocked.Contains(BrainRegion.Somatosensory))
                    return BrainRegion.Somatosensory;
                if (!unlocked.Contains(BrainRegion.VisualCortex))
                    return BrainRegion.VisualCortex;
                if (!unlocked.Contains(BrainRegion.Prefrontal))
                    return BrainRegion.Prefrontal;
            }

            // Default to Brainstem
            return BrainRegion.Brainstem;
        }

        /// <summary>
        /// Light up all previously unlocked regions (no particles, instant)
        /// </summary>
        private IEnumerator LightUpPreviousRegions(BrainRegion excludeRegion)
        {
            if (ProgressionManager.Instance == null)
                yield break;

            var unlockedRegions = ProgressionManager.Instance.GetUnlockedRegions();

            foreach (var region in unlockedRegions)
            {
                if (region != excludeRegion && region != BrainRegion.None)
                {
                    // Light up instantly (no animation)
                    if (regionLights.ContainsKey(region) && regionLights[region] != null)
                    {
                        regionLights[region].intensity = lightIntensity;
                    }
                }
            }

            yield return null;
        }

        /// <summary>
        /// Light up a specific brain region with optional particles
        /// </summary>
        private IEnumerator LightUpRegion(BrainRegion region, bool useParticles)
        {
            if (region == BrainRegion.None)
                yield break;

            // Play unlock sound
            if (regionUnlockSound != null && dramaticMusicSource != null)
            {
                dramaticMusicSource.PlayOneShot(regionUnlockSound);
            }

            // Start particles
            if (useParticles && regionParticles.ContainsKey(region) && regionParticles[region] != null)
            {
                regionParticles[region].Play();
            }

            // Fade in the light
            if (regionLights.ContainsKey(region) && regionLights[region] != null)
            {
                yield return StartCoroutine(FlickerLight(regionLights[region], lightActivationDuration, lightIntensity, 0.2f));
                //Light light = regionLights[region];
                //float elapsed = 0f;

                //while (elapsed < lightActivationDuration)
                //{
                //    elapsed += Time.deltaTime;
                //    float t = elapsed / lightActivationDuration;
                //    light.intensity = Mathf.Lerp(0f, lightIntensity, t);
                //    yield return null;
                //}

                //light.intensity = lightIntensity;
            }
        }

        /// <summary>
        /// Fade screen to black and transition to next scene
        /// </summary>
        private IEnumerator FadeOutAndTransition()
        {
            // Fade to black
            if (screenFlash != null)
            {
                bool fadeComplete = false;
                screenFlash.FadeToVoid(fadeOutDuration, () => fadeComplete = true);

                while (!fadeComplete)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(fadeOutDuration);
            }

            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelComplete();
            }

            // Load next scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }

        /// <summary>
        /// Set the transparency of the brain material
        /// </summary>
        private void SetBrainTransparency(float alpha)
        {
            if (brainMaterial == null) return;

            // Enable transparency on the material
            brainMaterial.SetFloat("_Surface", 1); // Transparent
            brainMaterial.SetFloat("_Blend", 0); // Alpha blend

            // Set rendering mode properties
            brainMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            brainMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            brainMaterial.SetInt("_ZWrite", 0);
            brainMaterial.renderQueue = 3000;

            // Set alpha
            Color color = brainMaterial.color;
            color.a = alpha;
            brainMaterial.color = color;

            // Also set base color if using URP
            if (brainMaterial.HasProperty("_BaseColor"))
            {
                brainMaterial.SetColor("_BaseColor", color);
            }
        }
    }
}
