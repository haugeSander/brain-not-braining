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

    private bool isPaused = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure menu starts hidden
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    private void Update()
    {
        // Toggle pause menu with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
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

    public void RestartLevel()
    {
        if (pauseMenuUI == null) return;
        
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Unfreeze game
        
        // Re-lock cursor if your game uses first-person controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before loading
        SceneManager.LoadScene(mainMenuSceneName);
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
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
}