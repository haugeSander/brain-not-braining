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
        if (isFlashing && flashImage != null)
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
}
