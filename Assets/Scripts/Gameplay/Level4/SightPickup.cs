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

    [Tooltip("Sound to play when collected.")]
    public AudioClip pickupSound;

    private bool isCollected = false; // Safeguard flag

    private void Awake()
    {
        // Ensure the collider is a trigger so it doesn't block movement.
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If already collected, do nothing.
        if (isCollected) return;

        // Check if the object that entered the trigger is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // GetComponentInParent is more robust, in case the collider is on a child object
            PlayerSight playerSight = other.GetComponentInParent<PlayerSight>();
            if (playerSight != null)
            {
                isCollected = true; // Set flag to prevent re-triggering

                // Disable the collider immediately to prevent re-triggers
                GetComponent<Collider>().enabled = false;

                // If it's the player, increase their sight
                playerSight.IncreaseMaxSight(sightIncreaseAmount);

                // Notify the level manager using the singleton instance
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.OnPickupCollected();
                }

                // Play a collection effect if one is assigned
                if (collectionEffect != null)
                {
                    Instantiate(collectionEffect, transform.position, Quaternion.identity);
                }

                // Play the pickup sound using AudioHelper.PlaySFX (doesn't require the object to exist)
                if (pickupSound != null)
                {
                    AudioHelper.PlaySFX(pickupSound, transform.position);
                }

                // Hide the visual immediately
                if (GetComponent<Renderer>() != null)
                {
                    GetComponent<Renderer>().enabled = false;
                }

                // Remove the pickup from the scene
                Destroy(gameObject);
            }
        }
    }
}