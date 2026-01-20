using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a "blinking" effect that starts with mostly edge detection and grants
/// progressively longer periods of normal vision over time.
/// </naturally>
public class VisionBlinker : MonoBehaviour
{
    [Header("Blink Control")]
    [Tooltip("The duration of the edge detection phase between blinks of normal vision.")]
    public float edgeVisionDuration = 3.0f;
    [Tooltip("Time in seconds for the blend to transition between vision modes.")]
    public float blinkTransitionDuration = 0.1f;

    [Header("Vision Improvement")]
    [Tooltip("The starting duration for the flash of normal vision.")]
    public float minNormalVisionDuration = 0.1f;
    [Tooltip("The maximum duration for normal vision as it improves.")]
    public float maxNormalVisionDuration = 2.0f;
    [Tooltip("How much the normal vision duration increases after each blink cycle.")]
    public float durationIncreasePerBlink = 0.1f;

    // The shader property ID for _BlendAmount, cached for performance.
    private static readonly int BlendAmountID = Shader.PropertyToID("_BlendAmount");

    private float currentNormalVisionDuration;

    private void Start()
    {
        // Start with edge detection enabled.
        Shader.SetGlobalFloat(BlendAmountID, 1f);
        currentNormalVisionDuration = minNormalVisionDuration;
        StartCoroutine(BlinkCoroutine());
    }

    /// <summary>
    /// Coroutine that runs the infinite blinking loop.
    /// </summary>
    private IEnumerator BlinkCoroutine()
    {
        // The game starts in edge vision. The first action is to wait for the first blink.
        while (true)
        {
            // Phase 1: Stay in EDGE detection for its set duration.
            yield return new WaitForSeconds(edgeVisionDuration);

            // Phase 2: Animate the blend back to NORMAL vision.
            yield return AnimateBlend(1f, 0f, blinkTransitionDuration);

            // Phase 3: Stay in NORMAL vision for the (short and increasing) duration.
            yield return new WaitForSeconds(currentNormalVisionDuration);

            // Phase 4: Animate the blend back to full EDGE detection.
            yield return AnimateBlend(0f, 1f, blinkTransitionDuration);

            // After a full cycle, improve the NORMAL vision duration for the next time.
            currentNormalVisionDuration = Mathf.Min(currentNormalVisionDuration + durationIncreasePerBlink, maxNormalVisionDuration);
        }
    }

    /// <summary>
    /// Coroutine to animate the _BlendAmount shader property over a given duration.
    /// </summary>
    private IEnumerator AnimateBlend(float start, float end, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Calculate the current blend value using linear interpolation.
            float blend = Mathf.Lerp(start, end, timer / duration);
            Shader.SetGlobalFloat(BlendAmountID, blend);
            yield return null;
        }
        // Ensure the final blend value is precisely set upon completion.
        Shader.SetGlobalFloat(BlendAmountID, end);
    }

    /// <summary>
    /// Public method to be called by other scripts (e.g., an item pickup script).
    /// This will permanently disable the blinking effect.
    /// </summary>
    public void UnlockPermanentVision()
    {
        Debug.Log("Permanent vision unlocked!");
        StopAllCoroutines();
        Shader.SetGlobalFloat(BlendAmountID, 0f); // Ensure vision is normal
        this.enabled = false; // Disable this component completely
    }

    private void OnDestroy()
    {
        // When the object is destroyed (e.g., scene change), reset the blend amount.
        // This prevents the effect from persisting in other scenes.
        Shader.SetGlobalFloat(BlendAmountID, 0f);
    }
}
