using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using BrainNotBraining.Gameplay;
using BrainNotBraining.Core;

public class ReflexBreathPuzzle : PuzzleBase
{
    [Header("Heart Rate Settings")]
    private int currentHR = 0; // Runtime value - start at 0 (mouse coming to life)
    [SerializeField] private int targetHR = 500;
    [SerializeField] private int failThresholdHR = 0;
    [SerializeField] private float hrDecayRate = 1f; // HR lost per second
    private bool heartHasBeenAwakened = false; // Track if HR has ever been > 0

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
    [SerializeField] private float minPitch = 0.8f; // Raised from 0.5 to sound less distorted
    [SerializeField] private float maxPitch = 2.5f; // Lowered from 3.0 for less extreme pitch shift

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI hrText;
    [SerializeField] private Image hrProgressBar;
    [SerializeField] private BreathMeter breathMeter;
    [SerializeField] private GameObject tutorialPanel; // First button + text
    [SerializeField] private ScreenFlash screenFlash;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private BrainNotBraining.UI.GameOverUI gameOverUI;
    [SerializeField] private WinSequence winSequence;
    [SerializeField] private GameObject mouseTrail; // Mouse trail effect to hide on death

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
    private bool isLosingSequenceActive = false; // Prevent multiple lose triggers
    private bool isInLowHRDanger = false; // Track if in low HR danger zone (< 10)
    private bool hasCountedOverfillThisCycle = false; // Prevent counting overfill multiple times per breath

    // Breath cycle state tracking
    private enum BreathCycleState { WaitingForInhale, Inhaling, WaitingForExhale, Exhaling }
    private BreathCycleState breathCycleState = BreathCycleState.WaitingForInhale;
    private bool reachedTopThreshold = false; // Did they inhale past 70%?
    private bool releasedCorrectly = false; // Did they release in valid range?
    private float timeBarReachedZero = -1f; // When did the bar reach 0%? (-1 = hasn't reached yet)

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
            heartbeatSource.pitch = minPitch; // Start at minimum pitch (0.8)
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
                musicSource.volume = 0.03f; // Start very low, will increase with HR
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

        // Handle HR decay (faster decay if not breathing)
        hrDecayTimer += Time.deltaTime;
        if (hrDecayTimer >= 1f)
        {
            hrDecayTimer = 0;
            if (currentHR > 0)
            {
                // Calculate decay multiplier based on idle time
                float timeSinceLastBreath = Time.time - lastBreathActionTime;
                float decayMultiplier = 1f;

                // After 3 seconds of not breathing, decay accelerates
                if (timeSinceLastBreath > 3f)
                {
                    decayMultiplier = 2f; // Double decay rate
                }

                // After 5 seconds (when idle warning appears), decay even faster
                if (timeSinceLastBreath > 5f)
                {
                    decayMultiplier = 3f; // Triple decay rate
                }

                int decayAmount = Mathf.RoundToInt(hrDecayRate * decayMultiplier);
                ModifyHeartRate(-decayAmount);

                if (decayMultiplier > 1f)
                {
                    Debug.Log($"HR decay accelerated: -{decayAmount} (idle for {timeSinceLastBreath:F1}s, multiplier: {decayMultiplier}x)");
                }
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

        // Update music volume based on HR (increase from 0.03 to 0.4)
        if (musicSource != null && musicSource.isPlaying)
        {
            float volumePercent = Mathf.Clamp01(currentHR / (float)targetHR);
            musicSource.volume = Mathf.Lerp(0.03f, 0.4f, volumePercent); // Max 0.4 instead of 0.6
        }

        // Check for low HR danger zone
        CheckLowHRWarning();

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

        // Track if heart has been awakened (HR went above 0)
        if (currentHR > 0 && !heartHasBeenAwakened)
        {
            heartHasBeenAwakened = true;
            Debug.Log("Heart awakened! HR is now above 0.");
        }

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

    /// <summary>
    /// Marks a breath as irregular and triggers warning effects
    /// </summary>
    private void MarkBreathIrregular(string reason)
    {
        if (isLosingSequenceActive) return; // Don't mark if already losing

        consecutiveIrregularBreaths++;
        Debug.Log($"Irregular breath: {reason} (count: {consecutiveIrregularBreaths}/{irregularBreathThreshold})");

        if (breathMeter != null)
        {
            breathMeter.SetRhythmStatus(false);
            breathMeter.UpdateIrregularBreathCount(consecutiveIrregularBreaths, irregularBreathThreshold);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBreathWarning();
        }

        // Flash screen based on severity
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

    private void UpdateBreathState()
    {
        bool breathKeyHeld = Input.GetKey(breathKey);
        float currentFillPercent = Mathf.Clamp01(breathTimer / targetInhaleDuration);

        // Update last breath action time when player is actively breathing
        if (breathKeyHeld || wasBreathing)
        {
            lastBreathActionTime = Time.time;

            // Clear idle warning if it was showing
            if (isShowingIdleWarning)
            {
                if (breathMeter != null)
                {
                    breathMeter.SetIdleWarning(false);
                }
                if (screenFlash != null)
                {
                    screenFlash.StopFadeToBlack();
                }
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.StopMuffling(1f);
                }
                isShowingIdleWarning = false;
            }
        }

        // STATE MACHINE for full breath cycle validation
        switch (breathCycleState)
        {
            case BreathCycleState.WaitingForInhale:
                // Continue exhaling to 0 if still exhaling
                if (breathTimer > 0 && !breathKeyHeld)
                {
                    breathTimer -= Time.deltaTime / targetExhaleDuration;
                    breathTimer = Mathf.Max(breathTimer, 0);

                    // Update visual
                    if (breathMeter != null)
                    {
                        breathMeter.UpdateBreathFill(currentFillPercent);
                    }
                }

                // Waiting for player to start inhaling from low position
                if (breathKeyHeld && !wasBreathing)
                {
                    // Check if starting from valid low position
                    if (currentFillPercent <= 0.20f)
                    {
                        // Good start - begin inhaling
                        breathCycleState = BreathCycleState.Inhaling;
                        reachedTopThreshold = false;
                        releasedCorrectly = false;
                        hasCountedOverfillThisCycle = false;

                        if (AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlayBreathInhale();
                        }
                    }
                    else
                    {
                        // Started too high - irregular breath
                        MarkBreathIrregular("Started inhaling too late");
                        breathCycleState = BreathCycleState.Inhaling; // Still track it
                        reachedTopThreshold = false;
                        releasedCorrectly = false;
                        hasCountedOverfillThisCycle = true; // Already counted this as irregular

                        // Still play inhale sound
                        if (AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlayBreathInhale();
                        }
                    }
                }
                break;

            case BreathCycleState.Inhaling:
                if (breathKeyHeld)
                {
                    // Continue inhaling
                    breathTimer += Time.deltaTime;
                    float fillPercent = breathTimer / targetInhaleDuration;

                    // Check if reached top threshold (70-100%)
                    if (fillPercent >= 0.70f && fillPercent <= 1.0f)
                    {
                        reachedTopThreshold = true;
                    }

                    // Check for overfill
                    if (fillPercent > 1.1f && !hasCountedOverfillThisCycle && !isLosingSequenceActive)
                    {
                        MarkBreathIrregular("Overfilled breath");
                        hasCountedOverfillThisCycle = true;
                        breathTimer = targetInhaleDuration * 1.1f; // Cap timer
                    }

                    // Update visual
                    if (breathMeter != null)
                    {
                        breathMeter.UpdateBreathFill(fillPercent);
                    }
                }
                else
                {
                    // Released space - check if release was in valid range
                    if (currentFillPercent >= 0.70f && currentFillPercent <= 1.0f && reachedTopThreshold)
                    {
                        releasedCorrectly = true;
                    }
                    else if (!hasCountedOverfillThisCycle)
                    {
                        MarkBreathIrregular("Released at wrong time");
                        hasCountedOverfillThisCycle = true;
                    }

                    breathCycleState = BreathCycleState.Exhaling;

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayBreathExhale();
                    }
                }
                break;

            case BreathCycleState.Exhaling:
                // Exhaling - bar going down
                if (breathTimer > 0)
                {
                    breathTimer -= Time.deltaTime / targetExhaleDuration;
                    breathTimer = Mathf.Max(breathTimer, 0);
                }

                // Check if exhaled back to bottom threshold
                if (currentFillPercent <= 0.15f)
                {
                    // Completed full cycle - check if it was a good breath
                    if (reachedTopThreshold && releasedCorrectly && !hasCountedOverfillThisCycle)
                    {
                        // GOOD BREATH - reset irregular counter
                        consecutiveIrregularBreaths = 0;
                        if (breathMeter != null)
                        {
                            breathMeter.SetRhythmStatus(true);
                            breathMeter.UpdateIrregularBreathCount(0, irregularBreathThreshold);
                        }
                        Debug.Log("Good breath completed!");
                    }

                    breathCyclesCompleted++;
                    breathCycleState = BreathCycleState.WaitingForInhale;
                    hasCountedOverfillThisCycle = false;
                }

                // Update visual
                if (breathMeter != null)
                {
                    breathMeter.UpdateBreathFill(currentFillPercent);
                }

                // If player starts inhaling again before reaching bottom, start new cycle
                if (breathKeyHeld && !wasBreathing)
                {
                    breathCycleState = BreathCycleState.WaitingForInhale;
                    // Trigger the inhale logic again by falling through to next frame
                }
                break;
        }

        wasBreathing = breathKeyHeld;

        // Track when bar reaches 0% and start countdown from there
        if (breathCyclesCompleted >= 2 && currentFillPercent <= 0.01f && !breathKeyHeld)
        {
            // Mark when bar first reaches 0%
            if (timeBarReachedZero < 0)
            {
                timeBarReachedZero = Time.time;
                Debug.Log("Breath bar reached 0% - starting idle countdown");
            }

            // Calculate time since bar reached 0%
            float timeSinceZero = Time.time - timeBarReachedZero;

            // Trigger warning after idle threshold
            if (timeSinceZero >= idleBreathWarningTime && !isShowingIdleWarning)
            {
                if (breathMeter != null)
                {
                    breathMeter.SetIdleWarning(true);
                    isShowingIdleWarning = true;
                    Debug.Log($"Breath bar at 0% for {timeSinceZero:F1}s - showing breath warning!");

                    // Start fading to black
                    if (screenFlash != null)
                    {
                        screenFlash.StartFadeToBlack();
                    }

                    // Start audio muffling
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.StartMuffling(2f);
                        AudioManager.Instance.PlayBreathWarning(); // Play warning sound
                    }

                    if (cameraShake != null)
                    {
                        cameraShake.ShakeMedium(); // Shake camera once at start
                    }
                }
            }
        }
        else if (breathKeyHeld || currentFillPercent > 0.01f)
        {
            // Reset timer when player starts breathing or bar rises above 0%
            timeBarReachedZero = -1f;
        }
        // Screen continues to fade darker while player is idle (handled by ScreenFlash.Update)
    }

    /// <summary>
    /// Checks if HR is dangerously low and triggers warning effects
    /// </summary>
    private void CheckLowHRWarning()
    {
        // Only check if heart has been awakened and game is active
        if (!heartHasBeenAwakened || !gameStarted || isLosingSequenceActive) return;

        bool isLowHR = currentHR > 0 && currentHR < 10;

        // Entering danger zone
        if (isLowHR && !isInLowHRDanger)
        {
            isInLowHRDanger = true;
            Debug.Log("Low HR warning: Heart failing!");

            // Start screen fade to red-tinted darkness
            if (screenFlash != null)
            {
                screenFlash.StartFadeToBlack(); // Reuse the fade system
            }

            // Start audio muffling
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StartMuffling(2f);
            }

            // Camera shake for urgency
            if (cameraShake != null)
            {
                cameraShake.ShakeMedium();
            }

            // Update breath meter status
            if (breathMeter != null)
            {
                breathMeter.SetIdleWarning(true); // Show "!!! BREATHE !!!" or similar
            }
        }
        // Recovering from danger zone (but NOT if HR hit 0 - that's game over!)
        else if (!isLowHR && isInLowHRDanger && currentHR > 0)
        {
            isInLowHRDanger = false;
            Debug.Log("Low HR warning cleared: HR recovering");

            // Stop screen fade
            if (screenFlash != null)
            {
                screenFlash.StopFadeToBlack();
            }

            // Stop audio muffling
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopMuffling(1f);
            }

            // Clear breath meter warning
            if (breathMeter != null)
            {
                breathMeter.SetIdleWarning(false);
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
        // Don't check if already losing
        if (isLosingSequenceActive) return;

        // Only check blackout if game has actually started
        bool isBlackedOut = gameStarted && screenFlash != null && screenFlash.IsFullyBlack();

        // Only fail from HR if it drops back to 0 AFTER being awakened
        bool heartFailed = heartHasBeenAwakened && currentHR <= failThresholdHR;

        if (heartFailed || consecutiveIrregularBreaths >= irregularBreathThreshold || isBlackedOut)
        {
            // Trigger atmospheric lose sequence
            string reason = isBlackedOut ? "Full blackout (stopped breathing)" :
                            heartFailed ? "Heart stopped (HR dropped back to 0)" :
                            "Too many irregular breaths";
            OnPuzzleFailed(reason);
        }
    }

    /// <summary>
    /// Handles the atmospheric lose sequence: fade to void, audio fade, show game over UI
    /// </summary>
    private void OnPuzzleFailed(string reason)
    {
        if (isLosingSequenceActive) return; // Prevent multiple triggers

        isLosingSequenceActive = true;
        gameStarted = false; // Stop the game

        Debug.Log($"Puzzle failed! Reason: {reason}");

        // Clear any warning states
        if (screenFlash != null)
        {
            screenFlash.StopFadeToBlack(); // Cancel low HR warning fade
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMuffling(0f); // Immediately clear audio muffling
        }
        isInLowHRDanger = false;
        isShowingIdleWarning = false;

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelFailed();
        }

        // Hide mouse trail
        if (mouseTrail != null)
        {
            mouseTrail.SetActive(false);
        }

        // Stop spawning buttons, clear active buttons
        foreach (var button in activeButtons)
        {
            if (button != null) Destroy(button);
        }
        activeButtons.Clear();

        // Start death sequence with 1 second delay
        StartCoroutine(DeathSequenceCoroutine(reason));
    }

    private System.Collections.IEnumerator DeathSequenceCoroutine(string reason)
    {
        // 1 second pause before effects start
        yield return new WaitForSecondsRealtime(1f);

        // Trigger atmospheric effects (always fade, since we cleared any warning fades)
        if (screenFlash != null)
        {
            // Slow fade to complete darkness (3 seconds)
            screenFlash.FadeToVoid(3f, OnVoidFadeComplete);
        }
        else
        {
            // No screen flash - go straight to game over
            OnVoidFadeComplete();
        }

        // Fade out all audio except heartbeat
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeOutAllExceptHeartbeat(3f);
            AudioManager.Instance.EchoHeartbeat(); // Echo and fade the heartbeat too
        }
    }

    /// <summary>
    /// Called when void fade completes - shows game over UI then clears screen
    /// </summary>
    private void OnVoidFadeComplete()
    {
        Debug.Log("Void fade complete - showing game over UI");

        // Show the Game Over UI FIRST (while screen is still black for immersion)
        if (gameOverUI != null)
        {
            Debug.Log($"GameOverUI found: {gameOverUI.name}, active: {gameOverUI.gameObject.activeInHierarchy}");
            gameOverUI.Show();

            // Clear the black screen AFTER a short delay so UI can fade in properly
            StartCoroutine(ClearScreenAfterDelay(0.5f));
        }
        else
        {
            Debug.LogError("GameOverUI not assigned in ReflexBreathPuzzle Inspector! Drag the Game Over Panel to the GameOverUI field.");

            // Still clear screen if UI is missing
            if (screenFlash != null)
            {
                screenFlash.ClearScreen();
            }
        }
    }

    /// <summary>
    /// Clears the screen overlay after a delay
    /// </summary>
    private System.Collections.IEnumerator ClearScreenAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (screenFlash != null)
        {
            screenFlash.ClearScreen();
        }

        Debug.Log("Screen cleared - Game Over UI now visible");
    }

    public override void Solve()
    {
        base.Solve();
        gameStarted = false;

        Debug.Log("Puzzle solved! Starting win sequence...");

        // Clean up active buttons
        foreach (var button in activeButtons)
        {
            if (button != null) Destroy(button);
        }
        activeButtons.Clear();

        // Trigger win sequence (which will handle GameManager.OnLevelComplete)
        if (winSequence != null)
        {
            winSequence.StartSequence(BrainRegion.Brainstem);
        }
        else
        {
            Debug.LogWarning("WinSequence not assigned! Falling back to direct GameManager call.");

            // Fallback: direct call to GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelComplete();
            }
        }
    }

    protected override void CheckPuzzleConditions()
    {
        // Not needed for this puzzle (handled in Update)
    }
}
