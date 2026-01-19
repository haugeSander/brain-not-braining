using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A singleton pause menu manager that persists across scenes.
/// Press ESC to open/close the pause menu.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("The parent GameObject containing all pause menu UI elements.")]
    public GameObject pauseMenuUI;
    [Tooltip("The main menu panel (shown first when pausing).")]
    public GameObject mainPanel;
    [Tooltip("The settings panel (optional, shown when clicking Settings).")]
    public GameObject settingsPanel;

    [Header("Settings")]
    [Tooltip("The name of your main menu scene.")]
    public string mainMenuSceneName = "MainMenu";
    [Tooltip("Scenes where the pause menu should be disabled (e.g., main menu).")]
    public string[] disabledScenes = { "MainMenu" };

    private bool isPaused = false;
    private bool isPauseEnabled = true;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Subscribe to scene loaded events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure menu starts hidden
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        // Debug check
        if (settingsPanel == null) 
            Debug.LogWarning("PauseMenuManager: Settings Panel is not assigned! Assign it in the Inspector.");
    }

    private void Update()
    {
        // Only allow pausing if enabled (not in main menu)
        if (!isPauseEnabled) return;
        
        // Toggle pause menu with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If settings is open, close it first
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the current scene is in the disabled list
        isPauseEnabled = true;
        foreach (string disabledScene in disabledScenes)
        {
            if (scene.name == disabledScene)
            {
                isPauseEnabled = false;
                // Force close pause menu if it was open
                if (isPaused) Resume();
                Debug.Log($"Pause menu DISABLED in scene: {scene.name}");
                break;
            }
        }
        
        if (isPauseEnabled)
        {
            Debug.Log($"Pause menu ENABLED in scene: {scene.name}");
        }
    }

    public void Pause()
    {
        if (pauseMenuUI == null) return;
        
        isPaused = true;
        pauseMenuUI.SetActive(true);
        
        // Show main panel, hide settings
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        Time.timeScale = 0f; // Freeze game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        if (pauseMenuUI == null) return;
        
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Unfreeze game
        
        // Re-lock cursor if your game uses first-person controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        Debug.Log("OpenSettings called");
        
        if (settingsPanel == null)
        {
            Debug.LogError("Settings Panel is NULL! Make sure it's assigned in the Inspector.");
            return;
        }
        
        if (mainPanel != null) mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        
        Debug.Log($"Settings panel active state: {settingsPanel.activeSelf}");
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // CRITICAL: Reset time scale BEFORE loading scene
        isPaused = false;
        
        // Hide UI
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        
        // Re-lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before loading
        isPaused = false;
        
        // Hide UI
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        
        // Unlock cursor for main menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene events to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}