using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace BrainNotBraining.Core
{
    /// <summary>
    /// Manages the level selection UI panel.
    /// Dynamically creates buttons for each level based on unlock status.
    /// </summary>
    public class LevelSelectManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private Transform buttonContainer;  // Parent object for level buttons
        [SerializeField] private GameObject levelButtonPrefab;
        
        [Header("Level Configuration")]
        [SerializeField] private List<LevelData> allLevels = new List<LevelData>();
        
        [Header("Button Colors")]
        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;

        private void Start()
        {
            // Hide panel on start
            if (levelSelectPanel != null)
                levelSelectPanel.SetActive(false);
        }

        /// <summary>
        /// Opens the level selection panel and populates it with buttons.
        /// Called by the "Play From" button in MenuManager.
        /// </summary>
        public void OpenLevelSelectPanel()
        {
            if (levelSelectPanel == null)
            {
                Debug.LogError("Level Select Panel not assigned!");
                return;
            }

            levelSelectPanel.SetActive(true);
            PopulateLevelButtons();
        }

        /// <summary>
        /// Closes the level selection panel.
        /// </summary>
        public void CloseLevelSelectPanel()
        {
            if (levelSelectPanel != null)
                levelSelectPanel.SetActive(false);
        }

        /// <summary>
        /// Creates a button for each level in the list.
        /// </summary>
        private void PopulateLevelButtons()
        {
            // Clear existing buttons
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }

            // Sort levels by index to ensure chronological order
            allLevels.Sort((a, b) => a.levelIndex.CompareTo(b.levelIndex));

            // Create a button for each level
            foreach (LevelData level in allLevels)
            {
                CreateLevelButton(level);
            }
        }

        /// <summary>
        /// Creates and configures a single level button.
        /// </summary>
        private void CreateLevelButton(LevelData level)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();
            
            // Get text component (supports both TextMeshPro and legacy Text)
            TextMeshProUGUI tmpText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Text legacyText = buttonObj.GetComponentInChildren<Text>();

            // Set button text
            if (tmpText != null)
                tmpText.text = level.levelName;
            else if (legacyText != null)
                legacyText.text = level.levelName;

            // Check if level is unlocked
            bool isUnlocked = IsLevelUnlocked(level);

            // Configure button appearance and interactivity
            button.interactable = isUnlocked;
            
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = isUnlocked ? unlockedColor : lockedColor;
            }

            // Add click listener to load the level
            if (isUnlocked)
            {
                button.onClick.AddListener(() => LoadLevel(level));
            }

            // Optional: Add icon if provided
            if (level.levelIcon != null)
            {
                Image iconImage = buttonObj.transform.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = level.levelIcon;
                }
            }
        }

        /// <summary>
        /// Checks if a level should be unlocked based on progression.
        /// </summary>
        private bool IsLevelUnlocked(LevelData level)
        {
            if (ProgressionManager.Instance == null)
            {
                Debug.LogWarning("ProgressionManager not found! Unlocking all levels by default.");
                return true;
            }

            // First level (Brainstem) is always unlocked
            if (level.requiredRegion == BrainRegion.Brainstem)
                return true;

            // Check if the required region is unlocked
            return ProgressionManager.Instance.IsRegionUnlocked(level.requiredRegion);
        }

        /// <summary>
        /// Loads the selected level scene.
        /// </summary>
        private void LoadLevel(LevelData level)
        {
            Debug.Log($"Loading level: {level.levelName} ({level.sceneName})");
            
            // Optional: Set current level index in ProgressionManager
            if (ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.SetCurrentLevelIndex(level.levelIndex);
            }

            SceneManager.LoadScene(level.sceneName);
        }
    }
}