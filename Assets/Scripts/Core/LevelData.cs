using UnityEngine;

namespace BrainNotBraining.Core
{
    /// <summary>
    /// ScriptableObject that holds data for a single level.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "BrainNotBraining/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Information")]
        public string levelName;           // Display name (e.g., "Brainstem")
        public string sceneName;            // Unity scene name
        public int levelIndex;              // Order in progression (0, 1, 2, etc.)
        
        [Header("Unlock Requirements")]
        public BrainRegion requiredRegion;  // Which region must be unlocked to play this level
        
        [Header("Visual")]
        [TextArea(2, 4)]
        public string description;          // Optional description for UI
        public Sprite levelIcon;            // Optional icon for the button
    }
}