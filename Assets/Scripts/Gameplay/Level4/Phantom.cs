using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

/// <summary>
/// The AI for the Phantom enemy. It remains idle and invisible until the player
/// uses the special sight ability nearby, at which point it becomes visible and hostile.
/// </summary>
[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider))]
public class Phantom : MonoBehaviour
{
    public enum PhantomState { IDLE, ALERTED, ATTACKING }

    [Header("Behavior")]
    [Tooltip("The current state of the Phantom.")]
    public PhantomState currentState = PhantomState.IDLE;

    [Tooltip("The distance at which the Phantom will detect the player's sight.")]
    public float sightTriggerDistance = 15f;

    [Header("Components")]
    [Tooltip("The visual part of the phantom that will be enabled when alerted.")]
    public Renderer visuals;

    // Internal references
    private NavMeshAgent agent;
    private PlayerSight playerSight; // Reference to the player's sight script

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (visuals == null)
        {
            Debug.LogError("Phantom: Visuals renderer is not assigned!", this);
        }
    }

    private void Start()
    {
        // Find the player's sight component in the scene.
        // This is simple, but for a larger game, a more robust manager/service locator might be better.
        playerSight = FindObjectOfType<PlayerSight>();

        if (playerSight == null)
        {
            Debug.LogError("Phantom: Could not find PlayerSight component in the scene!", this);
        }

        // Start invisible and idle
        visuals.enabled = false;
        agent.enabled = false;
    }

    private void Update()
    {
        // State machine logic
        switch (currentState)
        {
            case PhantomState.IDLE:
                UpdateIdle();
                break;
            case PhantomState.ALERTED:
                UpdateAlerted();
                break;
            case PhantomState.ATTACKING:
                UpdateAttacking();
                break;
        }
    }

    private void UpdateIdle()
    {
        // If the player exists, is using their sight, and is within range...
        if (playerSight != null && playerSight.IsSeeing &&
            Vector3.Distance(transform.position, playerSight.transform.position) < sightTriggerDistance)
        {
            // ...transition to the ALERTED state.
            ChangeState(PhantomState.ALERTED);
        }
    }

    private void UpdateAlerted()
    {
        // Chase the player
        agent.SetDestination(playerSight.transform.position);

        // Simple transition to ATTACKING state if close enough
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            ChangeState(PhantomState.ATTACKING);
        }
    }

    private void UpdateAttacking()
    {
        // Placeholder for attack logic (e.g., deal damage to player)
        Debug.Log("Phantom is attacking!");

        // For now, just go back to chasing if the player moves away
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            ChangeState(PhantomState.ALERTED);
        }
    }

    private void ChangeState(PhantomState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        // Logic to run on entering a new state
        switch (currentState)
        {
            case PhantomState.IDLE:
                visuals.enabled = false;
                agent.enabled = false;
                break;
            case PhantomState.ALERTED:
                Debug.Log("A Phantom has seen you!");
                visuals.enabled = true;
                agent.enabled = true;
                // Could play a sound or particle effect here
                break;
            case PhantomState.ATTACKING:
                // Could play an attack animation/sound
                break;
        }
    }
}
