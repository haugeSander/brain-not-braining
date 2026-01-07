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
        [SerializeField] private string fallbackSceneName = "MainMenu"; // Scene to load if no next level

        [Header("Usability")]
        [SerializeField] private KeyCode skipCutsceneButton = KeyCode.S;


        private BrainRegion currentlyActivatingRegion = BrainRegion.None;
        private Dictionary<BrainRegion, Light> regionLights = new Dictionary<BrainRegion, Light>();
        private Dictionary<BrainRegion, ParticleSystem> regionParticles = new Dictionary<BrainRegion, ParticleSystem>();
        private ScreenFlash screenFlash;

        private bool isTransitioning = false;

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

            if (brainMaterial != null)
            {
                // Create a temporary copy of the material just for this run
                brainMaterial = new Material(brainMaterial);

                // Apply this new copy to the renderer(s)
                var renderers = brainModel.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers) r.material = brainMaterial;

                SetBrainTransparency(brainTransparency);
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

            // Check Input directly here
            if (Input.GetKeyDown(skipCutsceneButton))
            {
                HandleSkip();
            }
        }

        // Helper function to handle the logic safely
        private void HandleSkip()
        {
            // Safety: Don't skip if we are already fading out!
            if (isTransitioning) return;

            Debug.Log("Skipping Cutscene...");

            // Set flag so we don't trigger this twice
            isTransitioning = true;

            // Stop the activation sequence so it doesn't keep playing sounds/particles
            StopAllCoroutines();

            // Immediately start the fade out
            StartCoroutine(FadeOutAndTransition());
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

            // Clear the pending unlock now that it's been processed
            ProgressionManager.PendingUnlock = BrainRegion.None;

            // Hold display for viewing
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(FadeOutAndTransition());
        }

        /// <summary>
        /// Determine which region was just unlocked based on the pending unlock
        /// </summary>
        private BrainRegion DetermineNewlyUnlockedRegion()
        {
            // Check if there's a pending unlock set by the level that was just completed
            if (ProgressionManager.PendingUnlock != BrainRegion.None)
            {
                Debug.Log($"Using pending unlock: {ProgressionManager.PendingUnlock}");
                return ProgressionManager.PendingUnlock;
            }

            // Fallback: check what's NOT yet unlocked (for backwards compatibility)
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

            Debug.LogWarning("No pending unlock and all regions unlocked. Defaulting to Brainstem.");
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
        /// Determines the next scene to load based on the region that was just unlocked
        /// </summary>
        private string GetNextSceneName(BrainRegion unlockedRegion)
        {
            switch (unlockedRegion)
            {
                case BrainRegion.Brainstem:
                    return "Level_1_Motor_Cortex";
                case BrainRegion.MotorCortex:
                    return "Level_3_somatosensory";
                case BrainRegion.Somatosensory:
                    return "Level_4_visualcortex";
                case BrainRegion.VisualCortex:
                    return "Puzzle_6";
                case BrainRegion.Prefrontal:
                    return "Level_7_PrefrontalCortex";
                default:
                    Debug.LogWarning($"No next scene defined for region {unlockedRegion}, loading fallback");
                    return fallbackSceneName; // Game complete, return to main menu
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

            // Determine next scene based on unlocked region
            string nextScene = GetNextSceneName(currentlyActivatingRegion);
            Debug.Log($"Transitioning to next scene: {nextScene}");

            // Load next scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }

        /// <summary>
        /// Set the transparency of the brain material
        /// </summary>
        private void SetBrainTransparency(float alpha)
        {
            if (brainMaterial == null) return;

            // Use _BaseColor because that is what we named it in the Shader code above!
            if (brainMaterial.HasProperty("_BaseColor"))
            {
                Color color = brainMaterial.GetColor("_BaseColor");
                color.a = alpha;
                brainMaterial.SetColor("_BaseColor", color);
            }
        }
    }
}
