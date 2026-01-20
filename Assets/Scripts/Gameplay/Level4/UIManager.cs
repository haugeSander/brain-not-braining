using UnityEngine;
using UnityEngine.UI; // Required for Slider and Button
using UnityEngine.SceneManagement; // Required for Scene management
using TMPro; // Required for TextMeshPro

/// <summary>
/// Manages all UI elements for Level 4, such as the sight meter, pickup counter, and death screen.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Game UI Elements")]
    [Tooltip("The slider representing the remaining time for the sight ability.")]
    public Slider sightMeterSlider;

    [Tooltip("The text that displays how many pickups have been collected.")]
    public TextMeshProUGUI pickupCounterText;

    [Header("Death Screen Elements")]
    [Tooltip("The parent panel for the death screen UI.")]
    public GameObject deathScreenPanel;

    [Tooltip("The button that restarts the level.")]
    public Button restartButton;

    [Tooltip("The button that exits the game.")]
    public Button exitButton;

    [Header("Tutorial Elements")]
    [Tooltip("The text element used for displaying tutorial messages.")]
    public TextMeshProUGUI tutorialText;

    private void Start()
    {
        // Ensure the death screen and tutorial text are hidden at the start
        if (deathScreenPanel != null) deathScreenPanel.SetActive(false);
        if (tutorialText != null) tutorialText.gameObject.SetActive(false);

        // Add listeners to the buttons
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    public void UpdateSightMeter(float currentValue, float maxValue)
    {
        if (sightMeterSlider != null)
        {
            sightMeterSlider.maxValue = maxValue;
            sightMeterSlider.value = currentValue;
        }
    }

    public void UpdatePickupCount(int currentCount, int totalCount)
    {
        if (pickupCounterText != null)
        {
            pickupCounterText.text = $"OBJECTS: {currentCount} / {totalCount}";
        }
    }

    /// <summary>
    /// Activates the death screen UI.
    /// </summary>
    public void ShowDeathScreen()
    {
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Shows the tutorial text element with a given message.
    /// </summary>
    public void ShowTutorialText(string message)
    {
        if (tutorialText != null)
        {
            tutorialText.text = message;
            tutorialText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the tutorial text element.
    /// </summary>
    public void HideTutorialText()
    {
        if (tutorialText != null)
        {
            tutorialText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Reloads the currently active scene.
    /// </summary>
    private void RestartGame()
    {
        // Time.timeScale is reset to 1 by the LevelManager's Awake() on scene load.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    private void ExitGame()
    {
        Debug.Log("Exiting game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
