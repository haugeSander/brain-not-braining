// Location: Assets/Scripts/Core/BrainRegion.cs
using System.Collections.Generic;
using System;

namespace BrainNotBraining.Core
{
    /// <summary>
    /// Represents the different brain regions unlocked throughout the game.
    /// Each region corresponds to a level and grants new abilities.
    /// </summary>
    [Serializable]
    public enum BrainRegion
    {
        None = 0,
        Brainstem = 1,      // Level 0: Reflexes (button sequences)
        MotorCortex = 2,    // Level 1: Movement (WASD)
        Somatosensory = 3,  // Level 2: Touch/spatial awareness
        VisualCortex = 4,   // Level 3: Sight (B&W → Color)
        Prefrontal = 5      // Level 4: Reasoning/memory
    }

    /// <summary>
    /// Serializable data structure for saving/loading brain state
    /// </summary>
    [Serializable]
    public class BrainState
    {
        public List<BrainRegion> unlockedRegions = new List<BrainRegion>();
        public int currentLevelIndex = 0;

        public bool HasRegion(BrainRegion region)
        {
            foreach (var unlocked in unlockedRegions)
            {
                if (unlocked == region) return true;
            }
            return false;
        }
    }
}
