using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BrainNotBraining.Core;

namespace BrainNotBraining.Gameplay
{
    /// <summary>
    /// Level 0 puzzle: Player must repeat button sequences to "awaken" the brainstem.
    /// Example: Game shows "W, S, D" → Player presses W, S, D → Success!
    /// </summary>
    public class SequencePuzzle : PuzzleBase
    {
        // === PUZZLE CONFIGURATION ===
        [Header("Sequence Settings")]
        [Tooltip("Number of sequences player must complete to solve puzzle")]
        public int numberOfSequences = 3;

        [Tooltip("How many buttons in each sequence")]
        public int sequenceLength = 4;

        [Tooltip("Time delay (seconds) to show each button in the sequence")]
        public float buttonDisplayDuration = 1.0f;

        [Tooltip("Available buttons for sequences (W, A, S, D, Space)")]
        private readonly List<string> availableButtons = new List<string> { "W", "A", "S", "D", "Space" };

        // === SEQUENCE STATE ===
        private List<string> currentSequence = new List<string>(); // The sequence player must repeat
        private List<string> playerInput = new List<string>();      // What player has entered so far
        private int completedSequences = 0;                          // How many sequences completed
        private bool isShowingSequence = false;                      // Are we displaying the sequence?
        private bool isWaitingForInput = false;                      // Is player's turn to input?


        // === UI REFERENCE ===
        [Header("UI")]
        [Tooltip("TextMeshPro component to display the sequence")]
        public TMPro.TextMeshProUGUI sequenceText;

        // === UNITY LIFECYCLE ===


        protected override void Start()
        {
            base.Start();

            // Start the heartbeat when Level 0 begins
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHeartbeat();
            }

            StartNextSequence();
        }

        // === PUZZLE LOGIC ===

        /// <summary>
        /// Generates a new random sequence and displays it to the player.
        /// </summary>
        private void StartNextSequence()
        {
            currentSequence.Clear();
            playerInput.Clear();

            for (int i = 0; i < sequenceLength; i++)
            {
                int randomIndex = Random.Range(0, availableButtons.Count);
                currentSequence.Add(availableButtons[randomIndex]);
            }
            StartCoroutine(ShowSequence());
            Debug.Log("Starting new sequence...");
        }

        /// <summary>
        /// Coroutine that displays the sequence one button at a time.
        /// </summary>
        private System.Collections.IEnumerator ShowSequence()
        {
            isShowingSequence = true;
            isWaitingForInput = false;

            foreach (string button in currentSequence)
            {
                if (sequenceText != null)
                    sequenceText.text = button; // Show the button on screen
                Debug.Log($"Show button: {button}");

                yield return new WaitForSeconds(buttonDisplayDuration);
                if (sequenceText != null)
                    sequenceText.text = ""; // Clear after delay
                yield return new WaitForSeconds(0.3f);
            }
            isShowingSequence = false;
            isWaitingForInput = true;

            if (sequenceText != null)
                sequenceText.text = "Your turn!";
            Debug.Log("Your turn! Repeat the sequence.");


        }

        /// <summary>
        /// Called when player presses a sequence input key (W, A, S, D, Space).
        /// </summary>
        // public void OnSequenceInput(CallbackContext context)
        // {
        //     if (!isWaitingForInput) return;
        //     string pressedKey = context.control.name;
        //     pressedKey = pressedKey.ToUpper();
        //     if (pressedKey == "SPACE") pressedKey = "Space";

        //     playerInput.Add(pressedKey);
        //     Debug.Log($"Player pressed: {pressedKey}");

        //     CheckPuzzleConditions();
        // }

        public void OnSequenceInputSpace()
        {
            playerInput.Add("Space");
            CheckPuzzleConditions();
        }

        public void OnSequenceInputA()
        {
            playerInput.Add("A");
            CheckPuzzleConditions();
        }
        public void OnSequenceInputS()
        {
            playerInput.Add("S");
            CheckPuzzleConditions();
        }
        public void OnSequenceInputW()
        {
            playerInput.Add("W");
            CheckPuzzleConditions();
        }
        public void OnSequenceInputD()
        {
            playerInput.Add("D");
            CheckPuzzleConditions();
        }
        
       
        /// <summary>
        /// Checks if player's input matches the sequence so far.
        /// </summary>
        protected override void CheckPuzzleConditions()
        {
            for (int i = 0; i < playerInput.Count; i++)
            {
                if (playerInput[i] != currentSequence[i])
                {
                    Debug.Log("Wrong input! Try again.");
                    if (sequenceText != null)
                        sequenceText.text = "Wrong! Try again...";
                    Invoke(nameof(StartNextSequence), 1.5f);
                    isWaitingForInput = false;
                    return;
                }
            }
            if (playerInput.Count == currentSequence.Count)
                OnSequenceComplete();
        }

        /// <summary>
        /// Called when player successfully completes one sequence.
        /// </summary>
        private void OnSequenceComplete()
        {
            completedSequences++;
            Debug.Log($"Sequence complete! {completedSequences}/{numberOfSequences}");

            if (completedSequences >= numberOfSequences)
            {
                if (sequenceText != null)
                    sequenceText.text = "Brainstem Awakened!";
                Solve();
            }
            else
            {
                if (sequenceText)
                    sequenceText.text = "Correct!";
                Invoke(nameof(StartNextSequence), 2.0f);
                isWaitingForInput = false;
            }
        }

        public override void Solve()
        {
            base.Solve();
            isWaitingForInput = false;

            // Optional: Play a success sound effect when puzzle completes
            // You can add a success AudioClip field and call AudioManager.Instance.PlaySFX(successClip) here

            GameManager.Instance.OnLevelComplete();
            Debug.Log("Level 0 (Brainstem) awakened!");
        }
    }
}
