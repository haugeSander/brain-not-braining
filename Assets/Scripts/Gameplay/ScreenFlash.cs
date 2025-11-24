using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates full-screen flash effects for dramatic moments
/// </summary>
public class ScreenFlash : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.3f;

    private float flashTimer = 0f;
    private bool isFlashing = false;
    private Color targetColor;

    // Sustained fade state (for breath idle effect)
    private bool isFading = false;
    private float fadeProgress = 0f;
    private float fadeDuration = 2f; // Time to reach 80% black
    private float fullBlackDuration = 8f; // Total time to reach 100% black (game over)
    private float clearSpeed = 1.5f; // Speed multiplier for clearing
    private float idleTime = 0f; // Track how long we've been fading

    private void Start()
    {
        if (flashImage != null)
        {
            // Start completely transparent
            Color c = flashImage.color;
            c.a = 0f;
            flashImage.color = c;
        }
    }

    private void Update()
    {
        if (flashImage == null) return;

        // Handle quick flash effects
        if (isFlashing)
        {
            flashTimer += Time.deltaTime;
            float progress = flashTimer / flashDuration;

            // Fade in then out (ping-pong effect)
            float alpha = Mathf.Sin(progress * Mathf.PI);
            Color c = targetColor;
            c.a = alpha * targetColor.a; // Respect target alpha
            flashImage.color = c;

            if (progress >= 1f)
            {
                isFlashing = false;
                c.a = 0f;
                flashImage.color = c;
            }
        }
        // Handle sustained fade to black (for breath idle)
        else if (isFading)
        {
            // Track idle time
            idleTime += Time.deltaTime;

            // Calculate fade progress based on idle time
            // 0-2s: fade to 80% black
            // 2-8s: continue fading to 100% black (game over)
            if (idleTime <= fadeDuration)
            {
                // First phase: fade to 80% over 2 seconds
                fadeProgress = (idleTime / fadeDuration) * 0.8f;
            }
            else
            {
                // Second phase: fade from 80% to 100% over remaining time
                float extraTime = idleTime - fadeDuration;
                float extraDuration = fullBlackDuration - fadeDuration;
                float extraProgress = Mathf.Clamp01(extraTime / extraDuration);
                fadeProgress = 0.8f + (extraProgress * 0.2f); // 0.8 to 1.0
            }

            Color c = Color.black;
            c.a = fadeProgress;
            flashImage.color = c;
        }
        // Handle clearing the fade (when breathing resumes)
        else if (fadeProgress > 0f)
        {
            // Reset idle timer
            idleTime = 0f;

            // Gradually clear the blackness
            fadeProgress -= Time.deltaTime * clearSpeed;
            fadeProgress = Mathf.Max(0f, fadeProgress);

            Color c = Color.black;
            c.a = fadeProgress;
            flashImage.color = c;
        }
    }

    /// <summary>
    /// Flash the screen with a color
    /// </summary>
    public void Flash(Color color, float duration = -1f)
    {
        if (duration > 0)
        {
            flashDuration = duration;
        }

        targetColor = color;
        flashTimer = 0f;
        isFlashing = true;
    }

    /// <summary>
    /// Flash white (danger/warning)
    /// </summary>
    public void FlashWhite()
    {
        Flash(new Color(1, 1, 1, 0.6f), 0.2f);
    }

    /// <summary>
    /// Flash red (damage/irregular breath)
    /// </summary>
    public void FlashRed()
    {
        Flash(new Color(1, 0, 0, 0.5f), 0.3f);
    }

    /// <summary>
    /// Flash black (losing consciousness)
    /// </summary>
    public void FlashBlack()
    {
        Flash(new Color(0, 0, 0, 0.7f), 0.4f);
    }

    /// <summary>
    /// Start fading to black (for breath idle warning)
    /// </summary>
    public void StartFadeToBlack()
    {
        isFading = true;
        isFlashing = false; // Cancel any flash in progress
    }

    /// <summary>
    /// Stop fading and gradually clear (when breathing resumes)
    /// </summary>
    public void StopFadeToBlack()
    {
        isFading = false;
        idleTime = 0f; // Reset idle timer
        // fadeProgress will naturally decrease in Update
    }

    /// <summary>
    /// Check if screen has reached full blackout (game over condition)
    /// </summary>
    public bool IsFullyBlack()
    {
        return fadeProgress >= 0.99f; // Treat 99%+ as fully black
    }
}
