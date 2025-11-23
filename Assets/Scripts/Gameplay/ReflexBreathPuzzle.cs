using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using BrainNotBraining.Gameplay;
using BrainNotBraining.Core;

public class ReflexBreathPuzzle : PuzzleBase
{
    [Header("Heart Rate Settings")]
    [SerializeField] private int currentHR = 0;
    [SerializeField] private int targetHR = 500;
    [SerializeField] private int failThresholdHR = 0;
    [SerializeField] private float hrDecayRate = 1f; // HR lost per second

    [Header("Spawn Settings")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private RectTransform spawnArea; // Canvas area bounds
    [SerializeField] private float initialSpawnInterval = 3f;
    [SerializeField] private float minimumSpawnInterval = 0.5f;
    [SerializeField] private int maxSimultaneousButtons = 5;
    [SerializeField] private AnimationCurve difficultyRamp; // Maps HR → spawn speed

    [Header("Breath Settings")]
    [SerializeField] private KeyCode breathKey = KeyCode.Space;
    [SerializeField] private float targetInhaleDuration = 3f;
    [SerializeField] private float targetExhaleDuration = 3f;
    [SerializeField] private float breathTolerancePercent = 0.3f; // 30% wiggle room
    [SerializeField] private int irregularBreathThreshold = 3; // 3 bad breaths = loss

    [Header("Audio")]
    [SerializeField] private AudioSource heartbeatSource;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 3f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI hrText;
    [SerializeField] private Image hrProgressBar;
    [SerializeField] private BreathMeter breathMeter;
    [SerializeField] private GameObject tutorialPanel; // First button + text

    private float spawnTimer;
    private bool gameStarted = false;
    private bool isInhaling = false;
    private float breathTimer;
    private float lastBreathEndTime;
    private int breathCyclesCompleted;
    private int consecutiveIrregularBreaths;
    private List<GameObject> activeButtons = new List<GameObject>();
    private float hrDecayTimer;

    // Methods to implement:
    // - Start(): Show tutorial, pause spawning
    // - OnTutorialButtonClick(): Hide tutorial, start game
    // - Update(): Handle spawning, breath input, HR decay, audio pitch
    // - SpawnButton(): Instantiate at random position within bounds
    // - ModifyHeartRate(int amount): Update HR, check win/loss
    // - UpdateBreathState(): Track inhale/exhale timing and quality
    // - CheckWinCondition(): HR >= 500 + breath quality threshold
    // - CheckLossCondition(): HR <= 0 OR irregular breath threshold

    protected override void Start()
    {
        base.Start();

        // Show tutorial panel
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        UpdateHRUI();

        // Start heartbeat audio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHeartbeat();
            heartbeatSource = AudioManager.Instance.heartbeatSource;
        }
    }

    public void OnTutorialButtonClick()
    {
        // Hide tutorial, start game
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        gameStarted = true;
        spawnTimer = initialSpawnInterval;
        lastBreathEndTime = Time.time;
    }

    private void Update()
    {
        if (!gameStarted || isSolved) return;

        // Handle button spawning
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && activeButtons.Count < maxSimultaneousButtons)
        {
            SpawnButton();

            // Calculate next spawn interval based on difficulty curve
            float progressPercent = Mathf.Clamp01(currentHR / (float)targetHR);
            float currentInterval = Mathf.Lerp(initialSpawnInterval, minimumSpawnInterval, difficultyRamp.Evaluate(progressPercent));
            spawnTimer = currentInterval;
        }

        // Handle HR decay
        hrDecayTimer += Time.deltaTime;
        if (hrDecayTimer >= 1f)
        {
            hrDecayTimer = 0;
            if (currentHR > 0)
            {
                ModifyHeartRate(-Mathf.RoundToInt(hrDecayRate));
            }
        }

        // Handle breath input
        UpdateBreathState();

        // Update heartbeat pitch
        if (heartbeatSource != null && currentHR > 0)
        {
            float pitchPercent = Mathf.Clamp01(currentHR / (float)targetHR);
            heartbeatSource.pitch = Mathf.Lerp(minPitch, maxPitch, pitchPercent);
        }

        // Check win/loss conditions
        CheckWinCondition();
        CheckLossCondition();
    }

    private void SpawnButton()
    {
        if (buttonPrefab == null || spawnArea == null) return;

        // Get random position within spawn area (use both width and height)
        float spawnWidth = spawnArea.rect.width * 0.8f; // 80% of width
        float spawnHeight = spawnArea.rect.height * 0.8f; // 80% of height

        Vector2 randomPos = new Vector2(
            Random.Range(-spawnWidth / 2f, spawnWidth / 2f),
            Random.Range(-spawnHeight / 2f, spawnHeight / 2f)
        );

        // Instantiate button
        GameObject button = Instantiate(buttonPrefab, spawnArea);
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        buttonRect.anchoredPosition = randomPos;

        // Ensure button is active and visible
        button.SetActive(true);

        activeButtons.Add(button);

        // Clean up destroyed buttons from list
        activeButtons.RemoveAll(b => b == null);

        Debug.Log($"Spawned button at position {randomPos}. Total active buttons: {activeButtons.Count}");
    }

    public void ModifyHeartRate(int amount)
    {
        currentHR = Mathf.Clamp(currentHR + amount, 0, targetHR + 100); // Allow slight overshoot
        UpdateHRUI();
    }

    private void UpdateHRUI()
    {
        if (hrText != null)
        {
            hrText.text = $"HR: {currentHR} / {targetHR}";
        }

        if (hrProgressBar != null)
        {
            hrProgressBar.fillAmount = Mathf.Clamp01(currentHR / (float)targetHR);
        }
    }

    private void UpdateBreathState()
    {
        bool breathKeyHeld = Input.GetKey(breathKey);

        if (breathKeyHeld)
        {
            breathTimer += Time.deltaTime;

            // Update breath meter visual
            if (breathMeter != null)
            {
                float fillPercent = Mathf.Clamp01(breathTimer / targetInhaleDuration);
                breathMeter.UpdateBreathFill(fillPercent);
            }
        }
        else
        {
            // Exhale phase
            if (breathTimer > 0) // Just released
            {
                // Check if inhale was in acceptable range
                float tolerance = targetInhaleDuration * breathTolerancePercent;
                bool inhaleCorrect = Mathf.Abs(breathTimer - targetInhaleDuration) <= tolerance;

                // Check if exhale timing was correct (time since last exhale)
                float timeSinceLastBreath = Time.time - lastBreathEndTime;
                float exhaleTolerance = targetExhaleDuration * breathTolerancePercent;
                bool exhaleCorrect = Mathf.Abs(timeSinceLastBreath - targetExhaleDuration) <= exhaleTolerance || breathCyclesCompleted == 0;

                bool breathQualityGood = inhaleCorrect && exhaleCorrect;

                if (breathQualityGood)
                {
                    consecutiveIrregularBreaths = 0;
                    if (breathMeter != null)
                    {
                        breathMeter.SetRhythmStatus(true);
                    }
                }
                else
                {
                    consecutiveIrregularBreaths++;
                    if (breathMeter != null)
                    {
                        breathMeter.SetRhythmStatus(false);
                    }
                }

                breathCyclesCompleted++;
                lastBreathEndTime = Time.time;
                breathTimer = 0;
            }

            // Update breath meter to show exhale
            if (breathMeter != null)
            {
                breathMeter.UpdateBreathFill(0);
            }
        }
    }

    private void CheckWinCondition()
    {
        if (currentHR >= targetHR && consecutiveIrregularBreaths == 0 && breathCyclesCompleted >= 3)
        {
            Solve();
        }
    }

    private void CheckLossCondition()
    {
        if (currentHR <= failThresholdHR || consecutiveIrregularBreaths >= irregularBreathThreshold)
        {
            // Handle loss (restart puzzle or show fail screen)
            Debug.Log("Puzzle failed! HR: " + currentHR + ", Irregular breaths: " + consecutiveIrregularBreaths);
            // Could call GameManager to restart level
        }
    }

    public override void Solve()
    {
        base.Solve();
        gameStarted = false;

        // Clean up active buttons
        foreach (var button in activeButtons)
        {
            if (button != null) Destroy(button);
        }
        activeButtons.Clear();

        // Trigger level complete
        GameManager.Instance.OnLevelComplete();
    }

    protected override void CheckPuzzleConditions()
    {
        // Not needed for this puzzle (handled in Update)
    }
}
