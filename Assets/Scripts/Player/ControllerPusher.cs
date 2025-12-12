using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControllerPusher : MonoBehaviour
{
    [Header("Push Settings")]
    public float pushStrength = 4f;         // force multiplier
    public string pushableTag = "Pushable"; // tag for pushable objects
    public bool onlyPushIfGrounded = true;  // optional: only push when player is grounded

    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // This is called by CharacterController when it hits a collider
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (onlyPushIfGrounded && !cc.isGrounded) return;

        if (!hit.collider.CompareTag(pushableTag)) return;

        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        // Determine push direction (horizontal only)
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        if (pushDir.sqrMagnitude < 0.001f) return;

        // Apply impulse at contact point for natural sliding
        rb.AddForceAtPosition(pushDir.normalized * pushStrength, hit.point, ForceMode.Impulse);
    }
}
