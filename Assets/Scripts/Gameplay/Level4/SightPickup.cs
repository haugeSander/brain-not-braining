using UnityEngine;

/// <summary>
/// A simple pickup item that increases the player's maximum sight duration
/// when the player walks into it.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SightPickup : MonoBehaviour
{
    [Header("Pickup Configuration")]
    [Tooltip("The amount of time to add to the player's maximum sight duration.")]
    public float sightIncreaseAmount = 0.5f;

    [Tooltip("Effect to play when the pickup is collected.")]
    public GameObject collectionEffect;

    // Reference to the level manager
    private LevelManager levelManager;

    private void Awake()
    {
        // Ensure the collider is a trigger so it doesn't block movement.
        GetComponent<Collider>().isTrigger = true;
    }

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the PlayerSight component
        PlayerSight playerSight = other.GetComponent<PlayerSight>();

        if (playerSight != null)
        {
            // If it's the player, increase their sight
            playerSight.IncreaseMaxSight(sightIncreaseAmount);

            // Notify the level manager
            if (levelManager != null)
            {
                levelManager.OnPickupCollected();
            }

            // Play a collection effect if one is assigned
            if (collectionEffect != null)
            {
                Instantiate(collectionEffect, transform.position, Quaternion.identity);
            }

            // Remove the pickup from the scene
            Destroy(gameObject);
        }
    }
}
