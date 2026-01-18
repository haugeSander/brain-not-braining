using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

/// <summary>
/// The AI for the Phantom enemy. It patrols a set route and remains invisible.
/// When the player uses the special sight ability nearby, it becomes visible and hostile.
/// </summary>
[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider))]
public class Phantom : MonoBehaviour
{
    public enum PhantomState { PATROLLING, ALERTED, ATTACKING }

    [Header("Behavior")]
    [Tooltip("The current state of the Phantom.")]
    public PhantomState currentState = PhantomState.PATROLLING;

    [Tooltip("The distance at which the Phantom will detect the player's sight.")]
    public float sightTriggerDistance = 15f;

    [Tooltip("A list of points for the Phantom to move between when not chasing the player.")]
    public Transform[] patrolPoints;

    [Header("Components")]
    [Tooltip("The visual part of the phantom that will be enabled when alerted.")]
    public Renderer visuals;

    [Header("Audio")]
    [Tooltip("Sound to play when the phantom catches the player.")]
    public AudioClip attackSound;
    [Tooltip("Volume of the attack sound.")]
    [Range(0f, 1f)]
    public float attackSoundVolume = 1.0f;

    // Internal references
    private NavMeshAgent agent;
    private PlayerSight playerSight;
    private int currentPatrolIndex = 0;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // Get the animator from the main object or its children
        animator = GetComponentInChildren<Animator>();
        if (visuals == null) Debug.LogError("Phantom: Visuals renderer is not assigned!", this);
    }

    private void Start()
    {
        playerSight = FindObjectOfType<PlayerSight>();
        if (playerSight == null) Debug.LogError("Phantom: Could not find PlayerSight component in the scene!", this);

        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"Phantom '{name}' is not on a valid NavMesh! Please ensure its starting position is on a baked NavMesh.", this);
        }

        ChangeState(PhantomState.PATROLLING);
    }

    private void Update()
    {
        switch (currentState)
        {
            case PhantomState.PATROLLING:
                UpdatePatrolling();
                break;
            case PhantomState.ALERTED:
                UpdateAlerted();
                break;
            case PhantomState.ATTACKING:
                // The attack logic is now handled once in ChangeState.
                break;
        }
    }

    private void UpdatePatrolling()
    {
        if (playerSight != null && playerSight.IsSeeing &&
            Vector3.Distance(transform.position, playerSight.transform.position) < sightTriggerDistance)
        {
            ChangeState(PhantomState.ALERTED);
            return;
        }

        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextPatrolPoint();
        }
    }

    private void GotoNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void UpdateAlerted()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(playerSight.transform.position);
            if (agent.remainingDistance < agent.stoppingDistance) ChangeState(PhantomState.ATTACKING);
        }
    }

    private void ChangeState(PhantomState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (currentState)
        {
            case PhantomState.PATROLLING:
                visuals.enabled = false;
                if(animator != null) animator.SetBool("isChasing", false);
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    GotoNextPatrolPoint();
                }
                break;
            case PhantomState.ALERTED:
                Debug.Log("A Phantom has seen you!");
                visuals.enabled = true;
                if(animator != null) animator.SetBool("isChasing", true);
                if (agent.isOnNavMesh) agent.isStopped = false;
                break;
            case PhantomState.ATTACKING:
                Debug.Log("Phantom caught the player!");
                if(animator != null) animator.SetTrigger("Attack");
                if (attackSound != null) AudioSource.PlayClipAtPoint(attackSound, transform.position, attackSoundVolume);
                if (agent.isOnNavMesh) agent.isStopped = true;
                if (LevelManager.Instance != null) LevelManager.Instance.TriggerPlayerDeath();
                break;
        }
    }
}
