using UnityEngine;
using System.Collections;
using BrainNotBraining.Core;
using BrainNotBraining.Gameplay;
using TMPro;

/// <summary>
/// Level 3 puzzle controller for somatosensory perception.
/// Manages puzzle activation, demo pulse, goal detection, and level completion.
/// Unlocks somatosensory abilities (echolocation, proximity reveal, temperature sensing).
/// </summary>
public class SomatosensoryPuzzle : PuzzleBase
{
    [Header("Somatosensory Configuration")]
    [Tooltip("Player GameObject (for finding systems)")]
    public GameObject player;

    [Tooltip("Goal area collider (basket/exit zone)")]
    public Collider goalAreaCollider;

    [Tooltip("Delay before triggering demo pulse (seconds)")]
    public float demoPulseDelay = 1.5f;

    [Tooltip("Delay after goal reached before loading next scene (seconds)")]
    public float completionDelay = 2.0f;

    [Tooltip("Display tutorial text on screen")]
    public bool showTutorialText = true;

    [Tooltip("Tutorial text UI component (TextMeshPro)")]
    public TextMeshProUGUI tutorialText;

    [Tooltip("Tutorial text display duration after second message (seconds)")]
    public float tutorialDuration = 5f;

    [Tooltip("Initial tutorial message")]
    public string initialTutorialMessage = "Press E, to focus sense";

    [Tooltip("Second tutorial message (after E pressed)")]
    public string secondTutorialMessage = "Sense your way to the prize";

    // References to player systems
    private EcholocationSystem echolocationSystem;
    private EcholocationPulseManager pulseManager;

    // State tracking
    private bool systemsActivated = true;
    private bool playerInGoal = false;
    private bool firstPulseTriggered = false;

    protected override void Start()
    {
        base.Start();

        // Find player if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            Debug.LogError("SomatosensoryPuzzle: No player found! Assign player GameObject or tag it as 'Player'.");
            return;
        }

        // Get player systems
        echolocationSystem = player.GetComponent<EcholocationSystem>();
        pulseManager = EcholocationPulseManager.Instance;

        // Validate goal area
        if (goalAreaCollider == null)
        {
            Debug.LogError("SomatosensoryPuzzle: No goal area collider assigned!");
        }
        else
        {
            // Ensure goal area is a trigger
            goalAreaCollider.isTrigger = true;
        }

        // Keep somatosensory systems enabled from start (no activation sequence)
        if (echolocationSystem != null)
        {
            echolocationSystem.enabled = true; // Changed: Keep enabled
        }

        if (pulseManager != null)
        {
            pulseManager.enabled = true; // Changed: Keep enabled
        }

        // Note: Activation sequence skipped - systems always enabled
        // If you want the "awakening" sequence, uncomment the line below:
        // ActivatePuzzle();

        // Show initial tutorial text
        if (showTutorialText && tutorialText != null)
        {
            ShowInitialTutorialText();
        }
    }

    private void Update()
    {
        // Check for first E press to update tutorial text
        if (!firstPulseTriggered && showTutorialText && tutorialText != null)
        {
            // Check if player has triggered a pulse
            if (echolocationSystem != null && pulseManager != null && pulseManager.GetActivePulseCount() > 0)
            {
                OnFirstPulseTriggered();
            }
        }
    }

    /// <summary>
    /// Activates the somatosensory puzzle - called when level starts.
    /// </summary>
    public override void ActivatePuzzle()
    {
        base.ActivatePuzzle();

        if (systemsActivated)
        {
            return; // Already activated
        }

        StartCoroutine(ActivationSequence());
    }

    /// <summary>
    /// Handles puzzle activation sequence with demo pulse.
    /// </summary>
    private IEnumerator ActivationSequence()
    {
        systemsActivated = true;

        Debug.Log("Somatosensory cortex awakening...");

        // Wait to let player experience darkness/limitation
        yield return new WaitForSeconds(demoPulseDelay);

        // Enable echolocation system
        if (echolocationSystem != null)
        {
            echolocationSystem.enabled = true;

            // Trigger demo pulse to introduce mechanic
            echolocationSystem.TriggerManualPulse();
            Debug.Log("Demo pulse triggered - revealing environment");
        }

        // Enable pulse manager for visual effects
        if (pulseManager != null)
        {
            pulseManager.enabled = true;
        }

        // Show tutorial text
        if (showTutorialText)
        {
            yield return new WaitForSeconds(0.5f);
            DisplayTutorialText();
        }

        Debug.Log("Somatosensory systems activated");
    }

    /// <summary>
    /// Shows initial tutorial text on screen.
    /// </summary>
    private void ShowInitialTutorialText()
    {
        if (tutorialText != null)
        {
            tutorialText.text = initialTutorialMessage;
            tutorialText.gameObject.SetActive(true);
            Debug.Log($"[TUTORIAL] Showing: {initialTutorialMessage}");
        }
    }

    /// <summary>
    /// Called when player triggers first pulse (presses E).
    /// Switches to second tutorial message.
    /// </summary>
    private void OnFirstPulseTriggered()
    {
        firstPulseTriggered = true;

        if (tutorialText != null)
        {
            tutorialText.text = secondTutorialMessage;
            Debug.Log($"[TUTORIAL] Showing: {secondTutorialMessage}");

            // Hide tutorial text after delay
            StartCoroutine(HideTutorialAfterDelay());
        }
    }

    /// <summary>
    /// Hides tutorial text after duration.
    /// </summary>
    private IEnumerator HideTutorialAfterDelay()
    {
        yield return new WaitForSeconds(tutorialDuration);

        if (tutorialText != null)
        {
            tutorialText.gameObject.SetActive(false);
            Debug.Log("[TUTORIAL] Text hidden");
        }
    }

    /// <summary>
    /// Displays tutorial text to player (legacy method for ActivatePuzzle sequence).
    /// </summary>
    private void DisplayTutorialText()
    {
        // If using ActivatePuzzle() sequence, show initial text
        ShowInitialTutorialText();
    }

    /// <summary>
    /// Checks if puzzle conditions are met (player reached goal).
    /// </summary>
    protected override void CheckPuzzleConditions()
    {
        if (isSolved || !systemsActivated)
        {
            return;
        }

        // Goal is reached when player enters goal area trigger
        // This is handled by OnTriggerEnter, so this method is passive
    }

    /// <summary>
    /// Handles player entering goal area.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (isSolved)
        {
            return;
        }

        if (other.CompareTag("Player") && !playerInGoal)
        {
            playerInGoal = true;
            OnGoalReached();
        }
    }

    /// <summary>
    /// Called when player reaches the goal area.
    /// </summary>
    private void OnGoalReached()
    {
        Debug.Log("Player reached goal area!");

        // Hide tutorial text immediately
        if (tutorialText != null)
        {
            tutorialText.gameObject.SetActive(false);
        }

        // Solve puzzle
        Solve();

        // Start completion sequence
        StartCoroutine(CompletionSequence());
    }

    /// <summary>
    /// Handles level completion sequence and scene transition.
    /// </summary>
    private IEnumerator CompletionSequence()
    {
        Debug.Log("Level 3 complete! Somatosensory cortex unlocked.");

        // Wait for completion delay
        yield return new WaitForSeconds(completionDelay);

        // Unlock somatosensory brain region
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.UnlockRegion(BrainRegion.Somatosensory);
            Debug.Log("Somatosensory region unlocked!");
        }

        // Transition to level complete state
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelComplete();

            // Load BrainRegionUnlocked scene
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.LoadLevel("BrainRegionUnlocked");
        }
        else
        {
            Debug.LogError("SomatosensoryPuzzle: GameManager not found! Cannot complete level properly.");

            // Fallback: load BrainRegionUnlocked scene directly
            UnityEngine.SceneManagement.SceneManager.LoadScene("BrainRegionUnlocked");
        }
    }

    /// <summary>
    /// Resets the puzzle to initial state.
    /// </summary>
    public override void ResetPuzzle()
    {
        base.ResetPuzzle();

        systemsActivated = true;
        playerInGoal = false;

        // Disable systems
        if (echolocationSystem != null)
        {
            echolocationSystem.enabled = false;
        }

        if (pulseManager != null)
        {
            pulseManager.enabled = false;
            pulseManager.ClearAllPulses();
        }
    }

    /// <summary>
    /// Returns true if somatosensory systems are active.
    /// </summary>
    public bool AreSystemsActivated()
    {
        return systemsActivated;
    }

    /// <summary>
    /// Manually triggers a demo pulse (for testing).
    /// </summary>
    [ContextMenu("Trigger Demo Pulse")]
    public void TriggerDemoPulse()
    {
        if (echolocationSystem != null)
        {
            echolocationSystem.TriggerManualPulse();
        }
    }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw goal area in editor
            if (goalAreaCollider != null)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
                Gizmos.matrix = goalAreaCollider.transform.localToWorldMatrix;

                if (goalAreaCollider is BoxCollider boxCol)
                {
                    Gizmos.DrawCube(boxCol.center, boxCol.size);
                }
                else if (goalAreaCollider is SphereCollider sphereCol)
                {
                    Gizmos.DrawSphere(sphereCol.center, sphereCol.radius);
                }

                UnityEditor.Handles.Label(goalAreaCollider.transform.position, "GOAL AREA");
            }
        }
#endif
}
