using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreathMeter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Image fillBar;
    [SerializeField] public TextMeshProUGUI statusText;
    [SerializeField] private Image validZoneTop; // Upper boundary line
    [SerializeField] private Image validZoneBottom; // Lower boundary line

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0, 1, 1); // Cyan
    [SerializeField] private Color inRhythmColor = Color.green;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private Color validZoneColor = new Color(0, 1, 0, 0.5f); // Semi-transparent green

    [Header("Warning Settings")]
    [SerializeField] private float blinkSpeed = 5f;
    [SerializeField] private Color overfilledColor = Color.yellow;
    [SerializeField] private TextMeshProUGUI irregularBreathCounter; // Shows "2/3" when irregular

    private bool isWarning = false;
    private bool isOverfilled = false;
    private bool isIdle = false; // Player not breathing
    private float blinkTimer = 0f;

    private void Start()
    {
        // Setup valid zone indicators if they exist
        if (validZoneTop != null)
        {
            validZoneTop.color = validZoneColor;
            // Ensure valid zone appears on top of fillbar by adjusting hierarchy
            validZoneTop.transform.SetAsLastSibling();
        }
        if (validZoneBottom != null)
        {
            validZoneBottom.color = validZoneColor;
            // Ensure valid zone appears on top of fillbar by adjusting hierarchy
            validZoneBottom.transform.SetAsLastSibling();
        }
    }

    private void Update()
    {
        // Handle warning blink animation (irregular breathing)
        if (isWarning && fillBar != null)
        {
            blinkTimer += Time.deltaTime * blinkSpeed;
            float alpha = Mathf.PingPong(blinkTimer, 1f);
            Color blinkColor = Color.Lerp(normalColor, warningColor, alpha);
            fillBar.color = blinkColor;
        }
        // Handle overfilled blink animation
        else if (isOverfilled && fillBar != null)
        {
            blinkTimer += Time.deltaTime * blinkSpeed * 2f; // Blink faster
            float alpha = Mathf.PingPong(blinkTimer, 1f);
            Color blinkColor = Color.Lerp(normalColor, overfilledColor, alpha);
            fillBar.color = blinkColor;
        }
        // Handle idle blink animation (player not breathing) - URGENT!
        else if (isIdle && fillBar != null)
        {
            blinkTimer += Time.deltaTime * blinkSpeed * 3f; // FAST blink for urgency!
            float alpha = Mathf.PingPong(blinkTimer, 1f);
            // Blink between red and orange for PANIC
            Color blinkColor = Color.Lerp(Color.red, new Color(1f, 0.5f, 0f), alpha);
            fillBar.color = blinkColor;
        }
    }

    public void UpdateBreathFill(float fillPercent)
    {
        // 0 = exhaled, 1 = fully inhaled
        if (fillBar != null)
        {
            // Cap at 1.0 and trigger overfilled warning if exceeding
            if (fillPercent > 1.0f)
            {
                fillBar.fillAmount = 1.0f;
                SetOverfilled(true);
            }
            else
            {
                fillBar.fillAmount = Mathf.Clamp01(fillPercent);
                SetOverfilled(false);
            }
        }
    }

    public void SetOverfilled(bool overfilled)
    {
        isOverfilled = overfilled;
        if (!overfilled && fillBar != null)
        {
            fillBar.color = normalColor;
            blinkTimer = 0f;
        }
    }

    public void SetRhythmStatus(bool inRhythm)
    {
        isWarning = !inRhythm;
        isIdle = false; // Clear idle state when actively breathing

        if (statusText != null)
        {
            statusText.text = inRhythm ? "In Rhythm" : "Irregular!";
            statusText.color = inRhythm ? inRhythmColor : warningColor;
        }

        // Reset fill bar color if back in rhythm
        if (inRhythm && fillBar != null)
        {
            fillBar.color = normalColor;
            blinkTimer = 0f;
        }
    }

    /// <summary>
    /// Sets idle warning when player hasn't breathed for a while
    /// </summary>
    public void SetIdleWarning(bool idle)
    {
        isIdle = idle;

        if (idle)
        {
            // Show URGENT text warning
            if (statusText != null)
            {
                statusText.text = "!BREATHE!";
                statusText.color = Color.red;
            }
            blinkTimer = 0f;
        }
        else
        {
            // Clear idle state
            if (fillBar != null)
            {
                fillBar.color = normalColor;
                blinkTimer = 0f;
            }

            // Clear text
            if (statusText != null)
            {
                statusText.text = "";
            }
        }
    }

    /// <summary>
    /// Sets the valid breath zone as percentage (0-1) of the bar
    /// </summary>
    public void SetValidZone(float minPercent, float maxPercent)
    {
        RectTransform parentRect = GetComponent<RectTransform>();
        if (parentRect == null) return;

        float barHeight = parentRect.rect.height;

        if (validZoneBottom != null)
        {
            RectTransform rt = validZoneBottom.GetComponent<RectTransform>();
            // Position at bottom of valid zone
            float yPosition = barHeight * minPercent - (barHeight / 2f);
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(1, 0.5f);
            rt.anchoredPosition = new Vector2(0, yPosition);
            rt.sizeDelta = new Vector2(0, 3); // 3 pixel line
        }

        if (validZoneTop != null)
        {
            RectTransform rt = validZoneTop.GetComponent<RectTransform>();
            // Position at top of valid zone
            float yPosition = barHeight * maxPercent - (barHeight / 2f);
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(1, 0.5f);
            rt.anchoredPosition = new Vector2(0, yPosition);
            rt.sizeDelta = new Vector2(0, 3); // 3 pixel line
        }
    }

    /// <summary>
    /// Updates the irregular breath counter display
    /// </summary>
    /// <param name="current">Current irregular breath count</param>
    /// <param name="max">Maximum before failure (usually 3)</param>
    public void UpdateIrregularBreathCount(int current, int max)
    {
        if (irregularBreathCounter == null)
        {
            Debug.LogWarning("BreathMeter: irregularBreathCounter not assigned! Drag the 'Irregular Counter Text' UI element to the BreathMeter Inspector.");
            return;
        }

        if (current <= 0)
        {
            // Hide counter when no irregular breaths
            irregularBreathCounter.text = "";
            irregularBreathCounter.gameObject.SetActive(false);
        }
        else
        {
            // Show counter with danger escalation
            irregularBreathCounter.gameObject.SetActive(true);
            irregularBreathCounter.text = $"{current}/{max}";

            // Color intensifies as you approach limit
            if (current == 1)
            {
                irregularBreathCounter.color = new Color(1f, 0.7f, 0f); // Orange warning
            }
            else if (current == 2)
            {
                irregularBreathCounter.color = new Color(1f, 0.3f, 0f); // Red-orange danger
            }
            else if (current >= max - 1)
            {
                irregularBreathCounter.color = Color.red; // Critical red
            }

            Debug.Log($"BreathMeter: Irregular breath count updated to {current}/{max}");
        }
    }
}
