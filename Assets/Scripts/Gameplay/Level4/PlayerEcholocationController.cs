using UnityEngine;

/// <summary>
/// This script acts as a bridge between PlayerMovement and EcholocationSystem.
/// It triggers echolocation pulses based on player actions (moving, manual trigger).
/// This component-based approach keeps the movement and echolocation systems decoupled.
/// </summary>
[RequireComponent(typeof(PlayerMovement), typeof(EcholocationSystem))]
public class PlayerEcholocationController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Key to trigger the manual echolocation pulse.")]
    public KeyCode manualPulseKey = KeyCode.E;

    [Tooltip("Enable to trigger pulses automatically while walking.")]
    public bool enableFootstepPulses = true;

    [Tooltip("The time interval between automatic footstep pulses while moving.")]
    [Range(0.1f, 2.0f)]
    public float footstepPulseInterval = 0.5f;

    // Component references
    private PlayerMovement playerMovement;
    private EcholocationSystem echolocationSystem;

    // Internal state
    private float lastFootstepPulseTime = -999f;

    private void Awake()
    {
        // Get references to the other components on this GameObject
        playerMovement = GetComponent<PlayerMovement>();
        echolocationSystem = GetComponent<EcholocationSystem>();
    }

    private void Update()
    {
        HandleManualPulse();
        HandleFootstepPulse();
    }

    /// <summary>
    /// Checks for the manual pulse key press and triggers the pulse.
    /// </summary>
    private void HandleManualPulse()
    {
        if (Input.GetKeyDown(manualPulseKey))
        {
            echolocationSystem.TriggerManualPulse();
        }
    }

    /// <summary>
    /// Checks if the player is moving and triggers footstep pulses at a set interval.
    /// </summary>
    private void HandleFootstepPulse()
    {
        if (!enableFootstepPulses)
        {
            return;
        }

        // Check the IsMoving() method on the PlayerMovement component
        if (playerMovement.IsMoving())
        {
            // Check if enough time has passed since the last pulse
            if (Time.time - lastFootstepPulseTime >= footstepPulseInterval)
            {
                echolocationSystem.TriggerFootstepPulse();
                lastFootstepPulseTime = Time.time;
            }
        }
    }
}
