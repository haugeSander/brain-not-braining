using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace BrainNotBraining.UI
{
    /// <summary>
    /// Manages the in-game overlay UI when player fails a puzzle.
    /// Displays mysterious/atmospheric failure message with retry options.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Settings")]
        [SerializeField] private float fadeInDuration = 1.5f;
        [SerializeField] private string[] failureMessages = new string[]
        {
            "Consciousness faded...",
            "The darkness returns...",
            "Lost to the void...",
            "Awakening failed..."
        };

        private bool isShowing = false;
        private float fadeProgress = 0f;

        private void Awake()
        {
            // Ensure UI starts hidden
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            // Wire up button callbacks
            if (retryButton != null)
            {
                retryButton.onClick.AddListener(OnRetry);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenu);
            }

            gameObject.SetActive(false);
        }

        private void Update()
        {
            // Handle fade-in animation
            if (isShowing && fadeProgress < 1f)
            {
                fadeProgress += Time.unscaledDeltaTime / fadeInDuration;
                fadeProgress = Mathf.Clamp01(fadeProgress);

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = fadeProgress;
                }

                // Enable interaction once fully visible
                if (fadeProgress >= 1f && canvasGroup != null)
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        /// <summary>
        /// Display the game over overlay with fade-in animation
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            isShowing = true;
            fadeProgress = 0f;

            // Set random failure message for variety
            if (messageText != null && failureMessages.Length > 0)
            {
                string message = failureMessages[Random.Range(0, failureMessages.Length)];
                messageText.text = message;
            }

            Debug.Log("GameOverUI: Showing overlay");
        }

        /// <summary>
        /// Hide the game over overlay immediately
        /// </summary>
        public void Hide()
        {
            isShowing = false;
            fadeProgress = 0f;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            gameObject.SetActive(false);
            Debug.Log("GameOverUI: Hidden");
        }

        /// <summary>
        /// Called when player clicks "Awaken Again" button
        /// </summary>
        private void OnRetry()
        {
            Debug.Log("GameOverUI: Retry button clicked - reloading scene");
            Hide();

            // Reload the current scene to restart the puzzle
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }

        /// <summary>
        /// Called when player clicks "Return to Menu" button
        /// </summary>
        private void OnMainMenu()
        {
            Debug.Log("GameOverUI: Main Menu button clicked");
            Hide();

            // Load main menu scene
            SceneManager.LoadScene("MainMenu");
        }
    }
}
