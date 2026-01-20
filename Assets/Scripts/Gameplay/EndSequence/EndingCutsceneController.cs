using UnityEngine;
using BrainNotBraining.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using BrainNotBraining.Gameplay;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace BrainNotBraining.Gameplay
{
    /// <summary>
    /// Controls the ending cutscene with three phases:
    /// 1. Full brain activation
    /// 2. Mouse escape from cage
    /// 3. Credits sequence
    /// </summary>
    public class EndingCutsceneController : MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField] private Camera brainCamera;
        [SerializeField] private Camera escapeCamera;
        [SerializeField] private Camera creditsCamera;

        [Header("Brain Section")]
        [SerializeField] private GameObject brainModel;
        [SerializeField] private Material brainMaterial;
        [SerializeField] private float brainInitialTransparency = 0.3f;
        [SerializeField] private float brainFinalTransparency = 1.0f;
        [SerializeField] private float brainRotationSpeed = 15f;

        [Header("Brain Particles")]
        [SerializeField] private ParticleSystem brainstemParticles;
        [SerializeField] private ParticleSystem motorCortexParticles;
        [SerializeField] private ParticleSystem somatosensoryParticles;
        [SerializeField] private ParticleSystem visualCortexParticles;
        [SerializeField] private ParticleSystem prefrontalParticles;

        [Header("Escape Section")]
        [SerializeField] private GameObject cage;
        [SerializeField] private GameObject cageDoor;
        [SerializeField] private Transform mouseCharacter;
        [SerializeField] private Animator mouseAnimator; // Optional: if you have walk animation
        [SerializeField] private string walkAnimationTrigger = "Walk"; // Animation trigger name

        [Header("Audio Mixer Groups")]
        [Tooltip("Assign your mixer groups for proper volume control")]
        public AudioMixerGroup musicMixerGroup;
        public AudioMixerGroup sfxMixerGroup;

        [Header("Audio")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip brainFinaleMusic;
        [SerializeField] private AudioClip escapeMusic;
        [SerializeField] private AudioClip creditsMusic;
        [SerializeField] private AudioClip regionActivationSound;
        [SerializeField] private AudioClip cageDoorSound;
        [SerializeField] private AudioClip mouseSqueak; // Optional cute touch

        [Header("UI")]
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private TextMeshProUGUI creditsText;
        [SerializeField] private TextMeshProUGUI skipHintText;

        [Header("Timing - Phase 1: Brain Finale")]
        [SerializeField] private float brainInitialDelay = 1f;
        [SerializeField] private float regionActivationDelay = 0.5f;
        [SerializeField] private float grandFinaleDelay = 2f;
        [SerializeField] private float grandFinaleHold = 5f;
        [SerializeField] private float brainPhaseTransitionDuration = 2f;

        [Header("Timing - Phase 2: Escape")]
        [SerializeField] private float cageDoorOpenDuration = 1.5f;
        [SerializeField] private float mouseWalkDuration = 4f;
        [SerializeField] private float mouseWalkDistance = 3f;
        [SerializeField] private float mouseHoldDuration = 2f;
        [SerializeField] private float escapePhaseTransitionDuration = 2f;

        [Header("Timing - Phase 3: Credits")]
        [SerializeField] private float creditsFadeInDuration = 1.5f;
        [SerializeField] private float creditsDisplayDuration = 15f;
        [SerializeField] private float creditsFadeOutDuration = 2f;
        [SerializeField] private string returnToScene = "MainMenu";
        [SerializeField] private float creditsScrollSpeed = 40f;
        [SerializeField] private float creditsEndPadding = 100f;
        [SerializeField] private float finalLineHoldDuration = 3f;


        [Header("Controls")]
        [SerializeField] private KeyCode skipButton = KeyCode.S;

        [Header("Credits Content")]
        [TextArea(10, 20)]
        [SerializeField] private string creditsContent = @"<size=72><b>Brain Not Braining</b></size>

<size=48>A Game About Discovery</size>



<size=36><b>Created By</b></size>
<size=32>Sander Hauge</size>
<size=32>Christoph Auer</size>
<size=32>Giulio Francesco Zemignani</size>



<size=36><b>Programming</b></size>
<size=28>Giulio Francesco Zemignani</size>



<size=36><b>Art & Design</b></size>
<size=28>Sander Hauge</size>



<size=36><b>Sound & Music</b></size>
<size=28>pixabay</size>



<size=36><b>Special Thanks</b></size>
<size=28>Everyone who supported this project</size>



<size=48><b>Thank You For Playing!</b></size>


<size=24>The mouse brain remembers...</size>";

        // Internal state
        private Dictionary<BrainRegion, ParticleSystem> regionParticles;
        private ScreenFlash screenFlash;
        private bool isTransitioning = false;
        private bool inCredits = false;
        private Camera currentCamera;

        private void Awake()
        {
            // Initialize particle dictionary
            regionParticles = new Dictionary<BrainRegion, ParticleSystem>
            {
                { BrainRegion.Brainstem, brainstemParticles },
                { BrainRegion.MotorCortex, motorCortexParticles },
                { BrainRegion.Somatosensory, somatosensoryParticles },
                { BrainRegion.VisualCortex, visualCortexParticles },
                { BrainRegion.Prefrontal, prefrontalParticles }
            };

            // Find ScreenFlash
            screenFlash = FindObjectOfType<ScreenFlash>();
            if (screenFlash == null)
            {
                Debug.LogError("ScreenFlash component not found! Please add it to the scene.");
            }

            // Setup brain material
            if (brainMaterial != null)
            {
                brainMaterial = new Material(brainMaterial);
                var renderers = brainModel.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers)
                {
                    r.material = brainMaterial;
                }
                SetBrainTransparency(brainInitialTransparency);
            }

            // Stop all particle systems
            foreach (var particles in regionParticles.Values)
            {
                if (particles != null)
                {
                    particles.Stop();
                }
            }

            // Setup cameras
            SetActiveCamera(brainCamera);

            // Hide credits UI
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }

            // Setup escape section (initially hidden)
            if (cageDoor != null)
            {
                cageDoor.SetActive(true);
            }

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.outputAudioMixerGroup = musicMixerGroup;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.outputAudioMixerGroup = sfxMixerGroup;
            }

        }

        private void Start()
        {
            StartCoroutine(RunEndingSequence());
        }

        private void Update()
        {
            // Brain rotation during phase 1
            if (currentCamera == brainCamera && brainModel != null)
            {
                brainCamera.transform.RotateAround(
                    brainModel.transform.position,
                    Vector3.up,
                    brainRotationSpeed * Time.deltaTime
                );
                brainCamera.transform.LookAt(brainModel.transform);
            }

            // Skip functionality
            if (Input.GetKeyDown(skipButton) && !isTransitioning)
            {
                HandleSkip();
            }
        }

        /// <summary>
        /// Main sequence controller
        /// </summary>
        private IEnumerator RunEndingSequence()
        {
            yield return StartCoroutine(Phase1_BrainFinale());
            yield return StartCoroutine(Phase2_MouseEscape());
            yield return StartCoroutine(Phase3_Credits());
        }

        #region Phase 1: Brain Finale

        private IEnumerator Phase1_BrainFinale()
        {
            Debug.Log("=== PHASE 1: Brain Finale ===");

            // Start dramatic music
            if (brainFinaleMusic != null && musicSource != null)
            {
                musicSource.clip = brainFinaleMusic;
                musicSource.Play();
            }

            yield return new WaitForSeconds(brainInitialDelay);

            // Activate regions in sequence (building up)
            BrainRegion[] regions = new BrainRegion[]
            {
                BrainRegion.Brainstem,
                BrainRegion.MotorCortex,
                BrainRegion.Somatosensory,
                BrainRegion.VisualCortex,
                BrainRegion.Prefrontal
            };

            foreach (var region in regions)
            {
                yield return StartCoroutine(ActivateRegion(region));
                yield return new WaitForSeconds(regionActivationDelay);
            }

            // Brief pause before grand finale
            yield return new WaitForSeconds(grandFinaleDelay);

            // GRAND FINALE - All regions fire at max intensity
            yield return StartCoroutine(GrandFinale());

            // Hold for dramatic effect
            yield return new WaitForSeconds(grandFinaleHold);

            // Transition to next phase
            yield return StartCoroutine(TransitionToEscapePhase());
        }

        private IEnumerator ActivateRegion(BrainRegion region)
        {
            if (region == BrainRegion.None || !regionParticles.ContainsKey(region))
                yield break;

            Debug.Log($"Activating region: {region}");

            // Play sound
            if (regionActivationSound != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(regionActivationSound);
            }

            // Start particle system
            ParticleSystem ps = regionParticles[region];
            if (ps != null)
            {
                ps.Play();

                // Also play any sub-emitters for neuron firing effect
                var subEmitters = ps.subEmitters;
                for (int i = 0; i < subEmitters.subEmittersCount; i++)
                {
                    ParticleSystem subPS = subEmitters.GetSubEmitterSystem(i);
                    if (subPS != null)
                    {
                        subPS.Play();
                    }
                }
            }
        }

        private IEnumerator GrandFinale()
        {
            Debug.Log("GRAND FINALE!");

            // Make brain fully opaque
            float elapsed = 0f;
            float duration = 1.5f;

            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(brainInitialTransparency, brainFinalTransparency, elapsed / duration);
                SetBrainTransparency(alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetBrainTransparency(brainFinalTransparency);

            // Boost all particle emission (if you have emission rate control)
            foreach (var ps in regionParticles.Values)
            {
                if (ps != null)
                {
                    var emission = ps.emission;
                    var rateOverTime = emission.rateOverTime;
                    rateOverTime.constant *= 2f; // Double the particles
                    emission.rateOverTime = rateOverTime;
                }
            }

            // Optional: Add screen flash effect
            if (screenFlash != null)
            {
                screenFlash.FlashWhite();
            }
        }

        private IEnumerator TransitionToEscapePhase()
        {
            // Fade to black
            if (screenFlash != null)
            {
                bool fadeComplete = false;
                screenFlash.FadeToVoid(brainPhaseTransitionDuration, () => fadeComplete = true);

                while (!fadeComplete)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(brainPhaseTransitionDuration);
            }

            // Switch camera
            SetActiveCamera(escapeCamera);

            // Fade from black
            if (screenFlash != null)
            {
                bool fadeComplete = false;
                screenFlash.FadeFromVoid(1f, () => fadeComplete = true);

                while (!fadeComplete)
                {
                    yield return null;
                }
            }
        }

        #endregion

        #region Phase 2: Mouse Escape

        private IEnumerator Phase2_MouseEscape()
        {
            Debug.Log("=== PHASE 2: Mouse Escape ===");

            // Change to softer escape music
            if (escapeMusic != null && musicSource != null)
            {
                musicSource.Stop();
                musicSource.clip = escapeMusic;
                musicSource.Play();
            }

            yield return new WaitForSeconds(0.5f);

            // Open cage door
            yield return StartCoroutine(OpenCageDoor());

            // Mouse walks out
            yield return StartCoroutine(MouseWalkOut());

            // Hold final pose
            yield return new WaitForSeconds(mouseHoldDuration);

            // Transition to credits
            yield return StartCoroutine(TransitionToCredits());
        }

        private IEnumerator OpenCageDoor()
        {
            if (cageDoor == null)
                yield break;

            Debug.Log("Opening cage door...");

            // Play door sound
            if (cageDoorSound != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(cageDoorSound);
            }

            // Animate door opening (rotate or slide)
            Vector3 startRotation = cageDoor.transform.localEulerAngles;
            Vector3 endRotation = startRotation + new Vector3(0, -90, 0); // Swing open

            float elapsed = 0f;

            while (elapsed < cageDoorOpenDuration)
            {
                cageDoor.transform.localEulerAngles = Vector3.Lerp(
                    startRotation,
                    endRotation,
                    elapsed / cageDoorOpenDuration
                );
                elapsed += Time.deltaTime;
                yield return null;
            }

            cageDoor.transform.localEulerAngles = endRotation;
        }

        private IEnumerator MouseWalkOut()
        {
            if (mouseCharacter == null)
                yield break;

            Debug.Log("Mouse walking to freedom...");

            // Optional: Play squeak sound
            if (mouseSqueak != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(mouseSqueak);
            }

            // Trigger walk animation if available
            if (mouseAnimator != null && !string.IsNullOrEmpty(walkAnimationTrigger))
            {
                mouseAnimator.SetTrigger(walkAnimationTrigger);
            }

            // Move mouse forward
            Vector3 startPos = mouseCharacter.position;
            Vector3 endPos = startPos + mouseCharacter.up * mouseWalkDistance;

            float elapsed = 0f;

            while (elapsed < mouseWalkDuration)
            {
                mouseCharacter.position = Vector3.Lerp(
                    startPos,
                    endPos,
                    elapsed / mouseWalkDuration
                );
                elapsed += Time.deltaTime;
                yield return null;
            }

            mouseCharacter.position = endPos;

            // Optional: Mouse looks back at camera
            yield return StartCoroutine(MouseLookBack());
        }

        private IEnumerator MouseLookBack()
        {
            if (mouseCharacter == null)
                yield break;

            Quaternion startRotation = mouseCharacter.rotation;
            Quaternion endRotation = Quaternion.LookRotation(escapeCamera.transform.position - mouseCharacter.position);

            float duration = 0.8f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                mouseCharacter.rotation = Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    elapsed / duration
                );
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator TransitionToCredits()
        {
            // Fade to black
            if (screenFlash != null)
            {
                bool fadeComplete = false;
                screenFlash.FadeToVoid(escapePhaseTransitionDuration, () => fadeComplete = true);

                while (!fadeComplete)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(escapePhaseTransitionDuration);
            }

            // Switch to credits camera
            SetActiveCamera(creditsCamera);

            // Hide skip hint (can't skip credits)
            if (skipHintText != null)
            {
                skipHintText.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Phase 3: Credits

        private IEnumerator Phase3_Credits()
        {
            Debug.Log("=== PHASE 3: Credits ===");
            inCredits = true;

            // Start credits music
            if (creditsMusic != null && musicSource != null)
            {
                musicSource.Stop();
                musicSource.clip = creditsMusic;
                musicSource.Play();
            }



            // Set credits text
            if (creditsText != null)
            {
                // Set text first
                creditsText.text = creditsContent;

                // Force layout & mesh update
                LayoutRebuilder.ForceRebuildLayoutImmediate(creditsText.rectTransform);
                creditsText.ForceMeshUpdate();

                // Move text off-screen BEFORE showing
                Canvas canvas = creditsText.GetComponentInParent<Canvas>();
                float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
                creditsText.rectTransform.anchoredPosition = new Vector2(0, -canvasHeight);
            }

            // Show credits panel
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(true);
            }


            // Fade from black
            if (screenFlash != null)
            {
                bool fadeComplete = false;
                screenFlash.FadeFromVoid(creditsFadeInDuration, () => fadeComplete = true);

                while (!fadeComplete)
                {
                    yield return null;
                }
            }

            // Fade in credits text
            yield return StartCoroutine(FadeInCreditsText());

            // Display credits
            yield return StartCoroutine(ScrollCreditsAndCenterFinalLine());

            // Fade out everything
            yield return StartCoroutine(FadeOutCredits());

            // Return to main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene(returnToScene);
        }

        private IEnumerator ScrollCreditsAndCenterFinalLine()
        {
            RectTransform rect = creditsText.rectTransform;

            // Force TMP to calculate size
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

            TMP_TextInfo textInfo = creditsText.textInfo;
            int lastLineIndex = textInfo.lineCount - 1;

            if (lastLineIndex < 0)
                yield break;

            TMP_LineInfo lastLine = textInfo.lineInfo[lastLineIndex];

            Canvas canvas = creditsText.GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            float canvasHeight = canvasRect.rect.height;

            float startY = -canvasHeight;
            rect.anchoredPosition = new Vector2(0, startY);

            // Position where last line is centered on screen
            float targetY =
                (canvasHeight / 2f)
                - lastLine.ascender
                - lastLine.baseline;

            while (rect.anchoredPosition.y < targetY)
            {
                rect.anchoredPosition += Vector2.up * creditsScrollSpeed * Time.deltaTime;
                yield return null;
            }

            // Snap exactly to center
            rect.anchoredPosition = new Vector2(0, targetY);

            // Hold final line
            yield return new WaitForSeconds(finalLineHoldDuration);
        }

        private IEnumerator FadeInCreditsText()
        {
            if (creditsText == null)
                yield break;

            float elapsed = 0f;
            float duration = creditsFadeInDuration;

            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(0, 1, elapsed / duration);
                creditsText.color = new Color(1, 1, 1, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            creditsText.color = Color.white;
        }

        private IEnumerator FadeOutCredits()
        {
            // Fade to black
            if (screenFlash != null)
            {
                bool fadeComplete = false;
                screenFlash.FadeToVoid(creditsFadeOutDuration, () => fadeComplete = true);

                while (!fadeComplete)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(creditsFadeOutDuration);
            }
        }

        #endregion

        #region Helper Methods

        private void SetActiveCamera(Camera camera)
        {
            brainCamera.gameObject.SetActive(false);
            escapeCamera.gameObject.SetActive(false);
            creditsCamera.gameObject.SetActive(false);

            if (camera != null)
            {
                camera.gameObject.SetActive(true);
                currentCamera = camera;
            }
        }

        private void SetBrainTransparency(float alpha)
        {
            if (brainMaterial == null)
                return;

            if (brainMaterial.HasProperty("_BaseColor"))
            {
                Color color = brainMaterial.GetColor("_BaseColor");
                color.a = alpha;
                brainMaterial.SetColor("_BaseColor", color);
            }
        }

        private void HandleSkip()
        {
            if (inCredits)
                return; // Don't skip credits

            if (isTransitioning)
                return;

            Debug.Log("Skipping cutscene...");
            isTransitioning = true;

            StopAllCoroutines();
            StartCoroutine(SkipToCredits());
        }

        private IEnumerator SkipToCredits()
        {
            // Quick fade
            if (screenFlash != null)
            {
                bool fadeComplete = false;
                screenFlash.FadeToVoid(0.5f, () => fadeComplete = true);

                while (!fadeComplete)
                {
                    yield return null;
                }
            }

            // Jump straight to credits
            SetActiveCamera(creditsCamera);
            yield return StartCoroutine(Phase3_Credits());
        }

        #endregion
    }
}