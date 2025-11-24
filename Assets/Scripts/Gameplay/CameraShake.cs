using UnityEngine;

/// <summary>
/// Simple camera shake effect for dramatic moments
/// </summary>
public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float dampingSpeed = 1.0f;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            // Random shake offset
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPosition;
        }
    }

    /// <summary>
    /// Trigger a camera shake
    /// </summary>
    /// <param name="duration">How long to shake (seconds)</param>
    /// <param name="magnitude">How intense the shake is</param>
    public void Shake(float duration, float magnitude)
    {
        originalPosition = transform.localPosition; // Update in case camera moved
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    /// <summary>
    /// Small shake (button click, small impact)
    /// </summary>
    public void ShakeSmall()
    {
        Shake(0.1f, 0.05f);
    }

    /// <summary>
    /// Medium shake (HR milestone, breath warning)
    /// </summary>
    public void ShakeMedium()
    {
        Shake(0.3f, 0.15f);
    }

    /// <summary>
    /// Large shake (major event, loss condition)
    /// </summary>
    public void ShakeLarge()
    {
        Shake(0.5f, 0.3f);
    }
}
