using UnityEngine;
using UnityEngine.SceneManagement;
using BrainNotBraining.Core;

namespace BrainNotBraining.Gameplay
{
    /// <summary>
    /// Orchestrates the win sequence:
    /// 1. White flash
    /// 2. Fade to white
    /// 3. Show brain visualization
    /// 4. Hold for viewing
    /// 5. Fade out
    /// 6. Transition to next level
    /// </summary>
    public class WinSequence : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScreenFlash screenFlash;
        [SerializeField] private BrainVisualization brainVisualization;

        [Header("Timing Settings")]
        [SerializeField] private float whiteFlashDuration = 0.5f;
        [SerializeField] private float fadeToWhiteDuration = 2f;
        [SerializeField] private float brainDisplayDuration = 5f;
        [SerializeField] private float fadeOutDuration = 1f;

        [Header("Scene Transition")]
        [SerializeField] private string nextSceneName = "LevelFinished";

        private bool isSequenceActive = false;

        /// <summary>
        /// Start the win sequence for a specific brain region
        /// </summary>
        public void StartSequence(BrainRegion unlockedRegion)
        {
            if (isSequenceActive)
            {
                Debug.LogWarning("WinSequence: Sequence already active!");
                return;
            }

            // Ensure this GameObject is active so coroutines can run
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            isSequenceActive = true;
            Debug.Log($"WinSequence: Starting for {unlockedRegion}");

            StartCoroutine(WinSequenceCoroutine(unlockedRegion));
        }

        private System.Collections.IEnumerator WinSequenceCoroutine(BrainRegion unlockedRegion)
        {
            // Step 1: Quick white flash for initial impact
            if (screenFlash != null)
            {
                screenFlash.FlashWhite();
                yield return new WaitForSecondsRealtime(whiteFlashDuration);
            }

            // Step 2: Fade to complete white
            if (screenFlash != null)
            {
                bool fadeComplete = false;
                screenFlash.FadeToWhite(fadeToWhiteDuration, () => fadeComplete = true);

                // Wait for fade to complete
                while (!fadeComplete)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSecondsRealtime(fadeToWhiteDuration);
            }

            // Step 3: Initialize and show brain visualization
            if (brainVisualization != null)
            {
                // Clear the white screen first
                if (screenFlash != null)
                {
                    screenFlash.ClearScreen();
                }

                // Initialize if not already done
                brainVisualization.Initialize();

                // Show the brain with region lit up
                string title = BrainVisualization.GetRegionTitle(unlockedRegion);
                string description = BrainVisualization.GetRegionDescription(unlockedRegion);
                brainVisualization.Show(unlockedRegion, title, description);

                // Hold for player to view
                yield return new WaitForSecondsRealtime(brainDisplayDuration);

                // Fade out brain visualization
                brainVisualization.Hide();
                yield return new WaitForSecondsRealtime(fadeOutDuration);
            }
            else
            {
                Debug.LogWarning("WinSequence: BrainVisualization not assigned!");
                yield return new WaitForSecondsRealtime(brainDisplayDuration);
            }

            // Step 4: Unlock region in progression system
            if (ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.UnlockRegion(unlockedRegion);
                Debug.Log($"WinSequence: Unlocked {unlockedRegion} in ProgressionManager");
            }

            // Step 5: Notify GameManager of level complete
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelComplete();
            }

            // Step 6: Transition to next scene
            yield return new WaitForSecondsRealtime(0.5f); // Brief pause before transition
            TransitionToNextScene();
        }

        /// <summary>
        /// Load the next scene (LevelFinished or next level)
        /// </summary>
        private void TransitionToNextScene()
        {
            Debug.Log($"WinSequence: Transitioning to {nextSceneName}");

            // Fade to black before scene transition
            if (screenFlash != null)
            {
                screenFlash.FadeToVoid(1f, () =>
                {
                    SceneManager.LoadScene(nextSceneName);
                });
            }
            else
            {
                // Fallback: immediate transition
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
