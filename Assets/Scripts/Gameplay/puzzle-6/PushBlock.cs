using UnityEngine;

public class PushBlock : MonoBehaviour
{
    public BlockColor blockColor;

    public void Consume()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        gameObject.SetActive(false); // hides block
    }
}
