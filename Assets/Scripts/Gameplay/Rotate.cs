using UnityEngine;

/// <summary>
/// Rotates the GameObject continuously around its vertical (Y) axis
/// Framerate independent rotation using Time.deltaTime (0.5 pts)
/// </summary>
public class Rotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 50f;
    
    void Update()
    {
        // Rotate around the Y-axis (vertical/up direction)
        // Time.deltaTime ensures rotation is framerate independent
        // This means the rotation speed is consistent regardless of FPS
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}