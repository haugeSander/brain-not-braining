using UnityEngine;
using System.Collections;
using BrainNotBraining.Core;

/// <summary>
/// Manages the overall state and objectives for Level 4.
/// Tracks pickup collection, win/loss conditions, and debug inputs.
/// Implemented as a singleton to ensure only one instance exists.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Game Objective")]
    [Tooltip("The total number of pickups that must be collected to win.")]
    public int totalPickupsInLevel = 0;
    [Tooltip("Delay after winning before loading the next scene.")]
    public float completionDelay = 2.0f;

    [Header("Tutorial")]
    [Tooltip("Should the tutorial text be displayed at the start?")]
    public bool showTutorial = true;
    [Tooltip("The tutorial message to display.")]
    [TextArea(3, 5)]
    public string tutorialMessage = "Hold [E] to see\nCollect all 5 objects\nAvoid the phantom";
    [Tooltip("How long the tutorial message stays on screen.")]
    public float tutorialDisplayDuration = 8f;

    [Header("Dependencies")]
    public PlayerMovement playerMovement;
    public ScreenFlash screenFlash;

    [Header("Cheat")]
    [Tooltip("Press this key to simulate collecting a pickup.")]
    public KeyCode cheatAddPickupKey = KeyCode.F;

    // Internal state
    private int pickupsCollected = 0;
    private bool isGameOver = false;
    private UIManager uiManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (playerMovement == null) playerMovement = FindObjectOfType<PlayerMovement>();
        if (screenFlash == null) screenFlash = FindObjectOfType<ScreenFlash>();
        
        totalPickupsInLevel = FindObjectsOfType<SightPickup>().Length;
        if (uiManager != null)
        {
            uiManager.UpdatePickupCount(pickupsCollected, totalPickupsInLevel);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (showTutorial)
        {
            StartCoroutine(TutorialSequence());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(cheatAddPickupKey))
        {
            Debug.Log("DEBUG: Manually collecting one pickup.");
            OnPickupCollected();
        }
    }

    public void OnPickupCollected()
    {
        if (isGameOver) return;
        pickupsCollected++;
        Debug.Log($"Pickup collected! Progress: {pickupsCollected}/{totalPickupsInLevel}");
        if (uiManager != null) uiManager.UpdatePickupCount(pickupsCollected, totalPickupsInLevel);
        if (pickupsCollected >= totalPickupsInLevel) WinGame();
    }

    public void TriggerPlayerDeath()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("GAME OVER: Player has died.");
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (playerMovement != null) playerMovement.enabled = false;
        if (uiManager != null) uiManager.ShowDeathScreen();
    }

    private void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("YOU WIN! All pickups collected.");
        StartCoroutine(CompletionSequence());
    }

    private IEnumerator CompletionSequence()
    {
        Debug.Log("Level 4 complete! Vision will be unlocked.");
        if (screenFlash != null)
        {
            bool fadeComplete = false;
            screenFlash.FadeToWhite(completionDelay, () => fadeComplete = true);
            while (!fadeComplete) yield return null;
        }
        else
        {
            yield return new WaitForSecondsRealtime(completionDelay);
        }
        ProgressionManager.PendingUnlock = BrainRegion.VisualCortex;
        Debug.Log("Pending unlock set to Vision");
        UnityEngine.SceneManagement.SceneManager.LoadScene("BrainRegionUnlocked");
    }

    private IEnumerator TutorialSequence()
    {
        // Wait a moment before showing the tutorial
        yield return new WaitForSeconds(1.5f);

        if (uiManager != null)
        {
            uiManager.ShowTutorialText(tutorialMessage);
        }

        // Wait for the specified duration
        yield return new WaitForSeconds(tutorialDisplayDuration);

        if (uiManager != null)
        {
            uiManager.HideTutorialText();
        }
    }
}
