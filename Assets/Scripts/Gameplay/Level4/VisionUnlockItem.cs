using UnityEngine;

public class VisionUnlockItem : MonoBehaviour
{
    [Tooltip("The tag of the player object.")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the player
        if (other.CompareTag(playerTag))
        {
            // Try to get the VisionBlinker component from the player object
            VisionBlinker blinker = other.GetComponent<VisionBlinker>();
            if (blinker != null)
            {
                // Call the public method to unlock permanent vision
                blinker.UnlockPermanentVision();

                // Optional: Destroy the unlock item after it has been collected
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Player entered trigger, but no VisionBlinker component was found on the player!");
            }
        }
    }
}
