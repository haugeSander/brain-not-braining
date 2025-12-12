using UnityEngine;
using UnityEngine.SceneManagement;

namespace BrainNotBraining.Core
{
    /// <summary>
    /// Central manager for game state and scene coordination.
    /// Singleton pattern ensures only one instance exists across scenes.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // === SINGLETON PATTERN ===
        // Static reference allows any script to access GameManager via GameManager.Instance        
        public static GameManager Instance { get; private set; }

        // === GAME STATE ===
        public enum GameState
        {
            MAINMENU,      // Not implemented yet
            PLAYING,       // Active gameplay
            LOADING,       // Loading between levels or similar
            PAUSED,        // Game paused
            OPTIONS,        // In the options menu
            LEVELCOMPLETE, // Level finished, transitioning
            GAMEOVER       // Player failed the level
        }

        private GameState _currentState;
        public GameState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                Debug.Log($"Game state changed to: {_currentState}");
            }
        }

        // === UNITY LIFECYCLE ===

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _currentState = GameState.PLAYING;
            Debug.Log("GameManager is ready");

            // Automatically load Level 0 for testing
            LoadLevel("Level_0_Brainstem");
        }

        // === PUBLIC API ===

        /// <summary>
        /// Loads a level by index. Called by ProgressionManager when unlocking new brain regions.
        /// </summary>
        public void LoadLevel(int levelIndex)
        {
            Debug.Log($"Loading level {levelIndex}");
            _currentState = GameState.LOADING;
            SceneManager.LoadScene(levelIndex);
            _currentState = GameState.PLAYING;
        }

        /// <summary>
        /// Loads a level by name. Useful for testing specific scenes.
        /// </summary>
        public void LoadLevel(string sceneName)
        {
            Debug.Log($"Loading level: {sceneName}");
            _currentState = GameState.LOADING;
            SceneManager.LoadScene(sceneName);
            _currentState = GameState.PLAYING;
        }

        /// <summary>
        /// Called when a level is completed. Triggers progression and next level load.
        /// </summary>
        public void OnLevelComplete()
        {
            _currentState = GameState.LEVELCOMPLETE;
            // Note: Region unlocking is now handled by BrainRegionActivator using PendingUnlock
            Debug.Log("Level complete!");
        }

        /// <summary>
        /// Called when a level is failed. Sets game over state.
        /// </summary>
        public void OnLevelFailed()
        {
            _currentState = GameState.GAMEOVER;
            // Don't pause Time.timeScale here - let the puzzle handle it
            // (allows animations and audio fades to continue)
            Debug.Log("Level failed - Game Over state active");
        }

        /// <summary>
        /// Pauses the game (sets Time.timeScale to 0).
        /// </summary>
        public void PauseGame()
        {
            Time.timeScale = 0f;
            _currentState = GameState.PAUSED;
        }

        /// <summary>
        /// Resumes the game (sets Time.timeScale back to 1).
        /// </summary>
        public void ResumeGame()
        {
            Time.timeScale = 1f;
            _currentState = GameState.PLAYING;
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void RestartLevel()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        /// <summary>
        /// Quits the application (works in builds, not in Editor).
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting game...");
            Application.Quit();

            // For testing in Unity Editor:
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
