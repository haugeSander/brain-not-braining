using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using BrainNotBraining.Gameplay;
using BrainNotBraining.Core;

public class ReflexBreathPuzzle : PuzzleBase
{
    [Header("Heart Rate Settings")]
    private int currentHR = 0; // Runtime value - NOT serialized to prevent Inspector interference
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
    [SerializeField] private float idleBreathWarningTime = 5f; // Show warning after 5 seconds of no breathing

    [Header("Audio")]
    [SerializeField] private AudioSource heartbeatSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip musicClip; // Assign your opus.mp3 here!
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 3f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI hrText;
    [SerializeField] private Image hrProgressBar;
    [SerializeField] private BreathMeter breathMeter;
    [SerializeField] private GameObject tutorialPanel; // First button + text
    [SerializeField] private ScreenFlash screenFlash;
    [SerializeField] private CameraShake cameraShake;

    private float spawnTimer;
    private bool gameStarted = false;
    private bool isInhaling = false;
    private bool wasBreathing = false; // Track breath state changes for SFX
    private float breathTimer;
    private float lastBreathActionTime; // Track when player last interacted with breathing
    private int breathCyclesCompleted;
    private int consecutiveIrregularBreaths;
    private int lastIrregularCount = 0; // Track when to flash screen
    private List<GameObject> activeButtons = new List<GameObject>();
    private float hrDecayTimer;
    private int lastMilestone = 0; // Track HR milestones for SFX
    private bool isShowingIdleWarning = false; // Track if idle warning is active

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

        // DEBUG: Ensure AudioManager exists when testing this scene directly
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager not found! Creating one for testing...");
            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManagerObj.AddComponent<AudioManager>();
        }
        else
        {
            InitializeAudio();
        }

        // Show tutorial panel (text set in Unity hierarchy)
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        UpdateHRUI();

        // Setup breath meter valid zones
        if (breathMeter != null)
        {
            // Bottom line: When to START pressing space (10-15%)
            // Top line: When to RELEASE space (80-100%)
            // Player should press space when bar is at bottom line, release when at top line
            float minValid = 0.15f; // Start pressing space here (15% of bar height)
            float maxValid = 0.85f; // Release space here (85% of bar height)
            breathMeter.SetValidZone(minValid, maxValid);
        }
    }

    private void InitializeAudio()
    {
        // Start heartbeat audio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHeartbeat();
            heartbeatSource = AudioManager.Instance.heartbeatSource;
            heartbeatSource.pitch = 0.4f;
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
        lastBreathActionTime = Time.time; // Initialize idle tracking

        // Start music
        if (AudioManager.Instance != null)
        {
            musicSource = AudioManager.Instance.musicSource;

            // Assign music clip if it's set in Inspector
            if (musicClip != null && musicSource != null)
            {
                musicSource.clip = musicClip;
                musicSource.loop = true;
                musicSource.volume = 0.3f;
                musicSource.Play();
                Debug.Log($"Music started directly: {musicClip.name}, Playing: {musicSource.isPlaying}");
            }
            else
            {
                if (musicClip == null)
                {
                    Debug.LogWarning("musicClip is not assigned in ReflexBreathPuzzle Inspector! Drag your music file to the 'Music Clip' field.");
                }
                else
                {
                    AudioManager.Instance.EnableMusic();
                    Debug.Log($"Music enabled via AudioManager. Playing: {musicSource != null && musicSource.isPlaying}");
                }
            }
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance is null! Cannot start music.");
        }
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
        int previousHR = currentHR;
        currentHR = Mathf.Clamp(currentHR + amount, 0, targetHR + 100); // Allow slight overshoot

        // Debug: Log HR changes
        Debug.Log($"HR Change: {previousHR} + ({amount}) = {currentHR}");

        // Check for HR milestones (every 100 HR)
        int currentMilestone = currentHR / 100;
        if (currentMilestone > lastMilestone && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHRMilestone();

            // Shake camera on milestone
            if (cameraShake != null)
            {
                cameraShake.ShakeMedium();
            }

            lastMilestone = currentMilestone;
        }

        UpdateHRUI();
    }

    private void UpdateHRUI()
    {
        if (hrText != null)
        {
            hrText.text = $"HR: {currentHR} / {targetHR}";
        }
        else
        {
            Debug.LogWarning("hrText is null! Cannot update HR display.");
        }

        if (hrProgressBar != null)
        {
            // Ensure fill amount starts at 0 when HR is 0
            float fillAmount = Mathf.Clamp01(currentHR / (float)targetHR);
            hrProgressBar.fillAmount = fillAmount;
        }
        else
        {
            Debug.LogWarning("hrProgressBar is null! Cannot update HR bar.");
        }
    }

    private void UpdateBreathState()
    {
        bool breathKeyHeld = Input.GetKey(breathKey);

        // Update last breath action time when player is actively breathing
        if (breathKeyHeld || wasBreathing)
        {
            lastBreathActionTime = Time.time;

            // Clear idle warning if it was showing
            if (isShowingIdleWarning && breathMeter != null)
            {
                breathMeter.SetIdleWarning(false);
                isShowingIdleWarning = false;
            }
        }

        if (breathKeyHeld)
        {
            // INHALE: Bar goes up
            // Play inhale sound on breath start
            if (!wasBreathing)
            {
                // Check if starting in valid range (below 20% is good)
                float currentFillPercent = Mathf.Clamp01(breathTimer / targetInhaleDuration);
                if (currentFillPercent > 0.20f)
                {
                    // Started breathing too late (bar is too high)
                    consecutiveIrregularBreaths++;
                    if (breathMeter != null)
                    {
                        breathMeter.SetRhythmStatus(false);
                    }
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayBreathWarning();
                    }
                    if (screenFlash != null)
                    {
                        screenFlash.FlashRed();
                    }
                }

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBreathInhale();
                }
            }
            wasBreathing = true;

            breathTimer += Time.deltaTime;

            // Check if holding too long (past maximum tolerance)
            float fillPercent = breathTimer / targetInhaleDuration;
            if (fillPercent > 1.1f) // 10% over the limit
            {
                // Mark as irregular for holding too long
                consecutiveIrregularBreaths++;
                if (breathMeter != null)
                {
                    breathMeter.SetRhythmStatus(false);
                }
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBreathWarning();
                }
                // Cap the timer to prevent extreme overfill
                breathTimer = targetInhaleDuration * 1.1f;
            }

            // Update breath meter visual
            if (breathMeter != null)
            {
                breathMeter.UpdateBreathFill(fillPercent);
            }
        }
        else
        {
            // EXHALE: Bar goes down slowly
            // Play exhale sound on breath release
            if (wasBreathing && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBreathExhale();

                // Check breath quality when releasing space
                // Player should release when bar is at 80-85% (top line)
                float currentFillPercent = Mathf.Clamp01(breathTimer / targetInhaleDuration);
                float targetRelease = 0.85f; // Should release around 85%
                float releaseTolerance = 0.15f; // 15% tolerance (so 70-100% is acceptable)
                bool releaseCorrect = Mathf.Abs(currentFillPercent - targetRelease) <= releaseTolerance;

                // For simplicity, consider breath good if released in correct range
                // (We could also check exhale timing, but let's keep it simple)
                bool breathQualityGood = releaseCorrect;

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

                    // Play warning sound and flash screen on irregular breath
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayBreathWarning();
                    }

                    // Flash screen on every irregular breath
                    if (screenFlash != null)
                    {
                        if (consecutiveIrregularBreaths == 1)
                        {
                            screenFlash.FlashRed(); // First warning
                        }
                        else if (consecutiveIrregularBreaths == 2)
                        {
                            screenFlash.FlashWhite(); // Second warning
                        }
                        else if (consecutiveIrregularBreaths >= irregularBreathThreshold - 1)
                        {
                            screenFlash.FlashBlack(); // About to lose!
                        }
                    }
                }

                breathCyclesCompleted++;
            }
            wasBreathing = false;

            // Slowly decrease breath timer (exhale)
            if (breathTimer > 0)
            {
                breathTimer -= Time.deltaTime / targetExhaleDuration;
                breathTimer = Mathf.Max(breathTimer, 0); // Don't go below 0
            }

            // Update breath meter to show current exhale state
            if (breathMeter != null)
            {
                float fillPercent = Mathf.Clamp01(breathTimer / targetInhaleDuration);
                breathMeter.UpdateBreathFill(fillPercent);
            }
        }

        // Check for idle breathing (player hasn't breathed in a while)
        float timeSinceLastBreath = Time.time - lastBreathActionTime;
        if (timeSinceLastBreath >= idleBreathWarningTime && !isShowingIdleWarning)
        {
            if (breathMeter != null)
            {
                breathMeter.SetIdleWarning(true);
                isShowingIdleWarning = true;
                Debug.Log($"Player idle for {timeSinceLastBreath:F1}s - showing breath warning!");

                // PANIC EFFECTS!
                if (screenFlash != null)
                {
                    screenFlash.FlashRed(); // Red screen flash
                }

                if (cameraShake != null)
                {
                    cameraShake.ShakeMedium(); // Shake camera
                }

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBreathWarning(); // Play warning sound
                }
            }
        }
        // Continue showing panic effects while idle
        else if (timeSinceLastBreath >= idleBreathWarningTime && isShowingIdleWarning)
        {
            // Flash and shake every 1.5 seconds while still idle
            if (Mathf.FloorToInt(timeSinceLastBreath) % 2 == 0 && Time.frameCount % 90 == 0)
            {
                if (screenFlash != null)
                {
                    screenFlash.FlashBlack(); // Escalate to black flash
                }

                if (cameraShake != null)
                {
                    cameraShake.ShakeMedium();
                }
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelComplete();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null! Cannot trigger level complete. Are you testing this scene directly?");
        }
    }

    protected override void CheckPuzzleConditions()
    {
        // Not needed for this puzzle (handled in Update)
    }
}
