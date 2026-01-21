using UnityEngine;

/// <summary>
/// Simple utility script that destroys the GameObject after a specified lifetime.
/// Useful for particle systems, temporary effects, and cleanup.
/// </summary>
public class AutoDestroy : MonoBehaviour
{
    [Tooltip("Time in seconds before this GameObject is destroyed")]
    public float lifetime = 0.5f;

    [Tooltip("Destroy children as well")]
    public bool destroyChildren = true;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Manually trigger destruction (useful for custom timing).
    /// </summary>
    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
