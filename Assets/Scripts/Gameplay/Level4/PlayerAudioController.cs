using UnityEngine;

/// <summary>
/// Manages audio feedback for player movement.
/// Plays a discrete footstep sound at a regular interval while the player is moving.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAudioController : MonoBehaviour
{
    [Header("Audio Configuration")]
    [Tooltip("The AudioSource to use for playing footstep sounds. The clip on this source will be used.")]
    public AudioSource movementAudio;

    [Tooltip("The time between each footstep sound while moving.")]
    [Range(0.1f, 1.5f)]
    public float footstepInterval = 0.4f;

    // Component references
    private PlayerMovement playerMovement;

    // Internal state
    private float lastFootstepTime = -999f;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (movementAudio == null)
        {
            Debug.LogWarning("PlayerAudioController: 'Movement Audio' source is not assigned. Movement sounds will not play.", this);
        }
        else if (movementAudio.clip == null)
        {
            Debug.LogWarning("PlayerAudioController: The assigned 'Movement Audio' source has no AudioClip. No sound will play.", this);
        }
    }

    private void Update()
    {
        if (movementAudio == null || movementAudio.clip == null) return;

        // Check that the player is both moving AND still providing input.
        // This prevents a final sound from playing while the player decelerates.
        bool hasInput = playerMovement.GetMoveInput().sqrMagnitude > 0.01f;

        if (playerMovement.IsMoving() && hasInput)
        {
            // Check if enough time has passed since the last footstep
            if (Time.time - lastFootstepTime >= footstepInterval)
            {
                // Play the sound once
                movementAudio.PlayOneShot(movementAudio.clip);
                lastFootstepTime = Time.time;
            }
        }
    }
}
