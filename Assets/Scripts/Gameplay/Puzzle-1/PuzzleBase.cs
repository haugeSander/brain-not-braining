using UnityEngine;
using UnityEngine.Events;

namespace BrainNotBraining.Gameplay
{
    /// <summary>
    /// Abstract base class for all puzzles in the game.
    /// Provides common functionality: state tracking, solve events, activation.
    /// </summary>
    public abstract class PuzzleBase : MonoBehaviour
    {
        private static readonly int BlendAmountID = Shader.PropertyToID("_BlendAmount");
        
        // === PUZZLE STATE ===
        [Header("Puzzle State")]
        [Tooltip("Is this puzzle currently solved?")]
        [SerializeField] protected bool isSolved = false;

        public bool IsSolved => isSolved;

        // === EVENTS ===
        [Header("Events")]
        [Tooltip("Called when puzzle is successfully solved")]
        public UnityEvent OnPuzzleSolved;

        // === UNITY LIFECYCLE ===

        protected virtual void Start()
        {
            // Initialize puzzle-specific logic
            // Override this in child classes if needed
            Shader.SetGlobalFloat(BlendAmountID, 0f);
        }

        // === PUBLIC API ===

        /// <summary>
        /// Activates the puzzle (called when player enters puzzle area or game starts).
        /// </summary>
        public virtual void ActivatePuzzle()
        {
            Debug.Log($"{gameObject.name} puzzle activated");
        }

        /// <summary>
        /// Marks the puzzle as solved and triggers events.
        /// </summary>
        public virtual void Solve()
        {
            if (isSolved) return;
            isSolved = true;
            OnPuzzleSolved?.Invoke();
            Debug.Log($"{gameObject.name} puzzle solved!");
        }

        /// <summary>
        /// Resets the puzzle to unsolved state (for testing or level restart).
        /// </summary>
        public virtual void ResetPuzzle()
        {
            isSolved = false;
        }

        // === ABSTRACT METHODS ===
        // These MUST be implemented by child classes

        /// <summary>
        /// Check if the puzzle conditions are met (called every frame or on input).
        /// Example: SequencePuzzle checks if button sequence is correct.
        /// </summary>
        protected abstract void CheckPuzzleConditions();
    }
}
