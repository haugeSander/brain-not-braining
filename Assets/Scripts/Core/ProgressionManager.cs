using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BrainNotBraining.Core
{
    /// <summary>
    /// Manages player progression through brain region unlocks.
    /// Handles save/load functionality using JSON serialization.
    /// </summary>
    public class ProgressionManager : MonoBehaviour
    {
        // === SINGLETON PATTERN ===
        public static ProgressionManager Instance { get; private set; }

        // === PROGRESSION STATE ===
        private BrainState _currentBrainState;

        // Save file path (persistent across game sessions)
        private string SaveFilePath => Path.Combine(Application.persistentDataPath, "save.json");

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
            _currentBrainState = LoadGame();
            if (_currentBrainState == null)
            {
                _currentBrainState = new BrainState();
                _currentBrainState.unlockedRegions = new List<BrainRegion> { BrainRegion.Brainstem };
                _currentBrainState.currentLevelIndex = 0;
            }
        }

        // === PUBLIC API ===

        /// <summary>
        /// Checks if a specific brain region is unlocked.
        /// </summary>
        public bool IsRegionUnlocked(BrainRegion region)
        {
            return _currentBrainState.unlockedRegions.Contains(region);
        }

        /// <summary>
        /// Unlocks a new brain region and saves progress.
        /// </summary>
        public void UnlockRegion(BrainRegion region)
        {
            if (!IsRegionUnlocked(region))
            {
                _currentBrainState.unlockedRegions.Add(region);
                Debug.Log($"Unlocked brain region: {region}");
                SaveGame();
            }
            else
            {
                Debug.Log($"{region} already unlocked!");
            }
        }

        /// <summary>
        /// Gets the current level index (used by GameManager for scene loading).
        /// </summary>
        public int GetCurrentLevelIndex()
        {
            return _currentBrainState.currentLevelIndex;
        }

        /// <summary>
        /// Sets the current level index when progressing.
        /// </summary>
        public void SetCurrentLevelIndex(int index)
        {
            _currentBrainState.currentLevelIndex = index;
            SaveGame();
        }

        /// <summary>
        /// Gets a list of all unlocked brain regions.
        /// </summary>
        public List<BrainRegion> GetUnlockedRegions()
        {
            return _currentBrainState.unlockedRegions;
        }

        // === SAVE/LOAD SYSTEM ===

        /// <summary>
        /// Saves the current brain state to a JSON file.
        /// </summary>
        public void SaveGame()
        {
            string json = JsonUtility.ToJson(_currentBrainState, true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"Game saved to: {SaveFilePath}");
        }

        /// <summary>
        /// Loads brain state from JSON file. Returns null if no save exists.
        /// </summary>
        public BrainState LoadGame()
        {
            if (!File.Exists(SaveFilePath)) return null;
            string json = File.ReadAllText(SaveFilePath);
            BrainState loadedState = JsonUtility.FromJson<BrainState>(json);
            Debug.Log("Loaded game files");
            return loadedState;
        }

        /// <summary>
        /// Deletes the save file (for testing or "New Game" option).
        /// </summary>
        public void DeleteSave()
        {
            if (File.Exists(SaveFilePath)) File.Delete(SaveFilePath);
            Debug.Log("Save file deleted");
            _currentBrainState.unlockedRegions = new List<BrainRegion> { BrainRegion.Brainstem };
            _currentBrainState.currentLevelIndex = 0;
        }

        // === ABILITY MANAGEMENT (For Player) ===

        /// <summary>
        /// Checks if the player can move (Motor Cortex unlocked).
        /// </summary>
        public bool CanPlayerMove()
        {
            return IsRegionUnlocked(BrainRegion.MotorCortex);
        }

        /// <summary>
        /// Checks if the player can see (Visual Cortex unlocked).
        /// </summary>
        public bool CanPlayerSee()
        {
            return IsRegionUnlocked(BrainRegion.VisualCortex);
        }

        /// <summary>
        /// Checks if the player has touch sense (Somatosensory Cortex unlocked).
        /// </summary>
        public bool HasTouchSense()
        {
            return IsRegionUnlocked(BrainRegion.Somatosensory);
        }
    }
}
