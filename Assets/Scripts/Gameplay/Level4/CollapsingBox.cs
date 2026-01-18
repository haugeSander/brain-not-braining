using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollapsingBox : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The tag of the object that will trigger the collapse (e.g., 'Player').")]
    public string triggerTag = "Player";

    [Tooltip("Delay in seconds before the box starts to fall after being touched.")]
    public float collapseDelay = 0.5f;

    [Tooltip("How long after collapsing the object should be destroyed. Set to 0 or less to disable.")]
    public float destroyDelay = 5.0f;

    [Header("Sound Effect")]
    [Tooltip("The single sound to play when the player first touches the box.")]
    public AudioClip collapseSound;

    private Rigidbody rb;
    private bool isTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Start with the box being kinematic (not affected by physics, stays in place)
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if we haven't been triggered yet and if the object that hit us has the correct tag
        if (!isTriggered && collision.gameObject.CompareTag(triggerTag))
        {
            isTriggered = true;

            // Play the collapse sound, if assigned
            if (collapseSound != null)
            {
                AudioSource.PlayClipAtPoint(collapseSound, transform.position);
            }

            // Start the collapse sequence after a short delay
            Invoke(nameof(Collapse), collapseDelay);
        }
    }

    private void Collapse()
    {
        // Make the box non-kinematic, so it will be affected by gravity and fall.
        rb.isKinematic = false;
        
        // Schedule the object's final destruction
        if (destroyDelay > 0)
        {
            Invoke(nameof(FinalizeDestruction), destroyDelay);
        }
    }

    private void FinalizeDestruction()
    {
        // The sound now plays on contact, so this method just destroys the object.
        Destroy(gameObject);
    }
}
