using UnityEngine;

/// <summary>
/// Manages the player's "special sight" ability.
/// Handles input, resource management (sight time), and transitions the edge detection effect.
/// </summary>
public class PlayerSight : MonoBehaviour
{
    [Header("Sight Parameters")]
    [Tooltip("The maximum duration the sight can be active.")]
    public float maxSightTime = 1.0f;

    [Tooltip("How quickly the sight drains when active (units per second).")]
    public float drainRate = 1.0f;

    [Tooltip("How quickly the sight recharges when not active (units per second).")]
    public float rechargeRate = 0.5f;

    [Tooltip("How quickly the vision transitions between states.")]
    public float transitionSpeed = 10f;

    [Header("Spam Prevention")]
    [Tooltip("Cooldown time after releasing E before it can be activated again.")]
    public float activationCooldown = 0.3f;

    [Header("Input")]
    [Tooltip("The key to hold to activate the sight ability.")]
    public KeyCode sightKey = KeyCode.E;

    // The shader property ID for _BlendAmount, cached for performance.
    private static readonly int BlendAmountID = Shader.PropertyToID("_BlendAmount");

    // Public state
    public bool IsSeeing { get; private set; }

    // Internal state
    private float currentSightTime;
    private float cooldownTimer = 0f;
    private bool wasKeyPressed = false;
    private UIManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        currentSightTime = maxSightTime;
        IsSeeing = false; // Start with sight disabled
        // Start in normal vision (0 = normal, 1 = edge detection)
        Shader.SetGlobalFloat(BlendAmountID, 1f);
    }

    private void Update()
    {
        HandleSightAbility();
        UpdateEdgeDetectionBlend();

        if (uiManager != null)
        {
            uiManager.UpdateSightMeter(currentSightTime, maxSightTime);
        }
    }

    private void OnDestroy()
    {
        // Cleanup: Reset to normal vision when destroyed
        Shader.SetGlobalFloat(BlendAmountID, 0f);
    }

    private void OnApplicationQuit()
    {
        // Reset to normal vision when exiting play mode in editor
        Shader.SetGlobalFloat(BlendAmountID, 0f);
    }

    private void OnDisable()
    {
        // Reset to normal vision when script is disabled (e.g., exiting play mode)
        Shader.SetGlobalFloat(BlendAmountID, 0f);
    }

    private void HandleSightAbility()
    {
        bool keyCurrentlyPressed = Input.GetKey(sightKey);
        
        // Update cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Detect key release to start cooldown
        if (wasKeyPressed && !keyCurrentlyPressed)
        {
            cooldownTimer = activationCooldown;
        }

        wasKeyPressed = keyCurrentlyPressed;

        // Determine if player wants to see (key pressed AND cooldown expired)
        bool wantsToSee = keyCurrentlyPressed && cooldownTimer <= 0;
        
        // Player is seeing if they want to AND have time remaining
        // OR if they're holding the key even after running out (prevents blinking)
        if (wantsToSee && currentSightTime > 0)
        {
            IsSeeing = true;
            // Drain the sight time
            currentSightTime -= drainRate * Time.deltaTime;
        }
        else if (wantsToSee && currentSightTime <= 0)
        {
            // Key is held but out of time - stay in edge detection mode
            IsSeeing = false;
            currentSightTime = 0; // Keep it at 0
        }
        else
        {
            // Key not held OR on cooldown - return to normal vision
            IsSeeing = false;
            // Recharge when not active
            if (currentSightTime < maxSightTime)
            {
                currentSightTime += rechargeRate * Time.deltaTime;
            }
        }

        // Clamp values to ensure they stay within bounds
        currentSightTime = Mathf.Clamp(currentSightTime, 0, maxSightTime);
    }

    private void UpdateEdgeDetectionBlend()
    {
        // 0 = full edge detection, 1 = normal vision
        // When IsSeeing is true, we want edge detection (0)
        float targetBlend = IsSeeing ? 0f : 1f;
        
        // Get current value to lerp from
        float currentBlend = Shader.GetGlobalFloat(BlendAmountID);

        // Smoothly transition the blend amount
        float newBlend = Mathf.Lerp(currentBlend, targetBlend, Time.deltaTime * transitionSpeed);

        // Snap to target if very close to avoid floating point precision issues
        if (Mathf.Abs(newBlend - targetBlend) < 0.001f)
        {
            newBlend = targetBlend;
        }

        Shader.SetGlobalFloat(BlendAmountID, newBlend);
    }

    /// <summary>
    /// Increases the maximum sight time. Called by pickups.
    /// </summary>
    public void IncreaseMaxSight(float amount)
    {
        maxSightTime += amount;
        Debug.Log($"Max sight time increased to {maxSightTime}");
    }
}