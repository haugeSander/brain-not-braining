using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreathMeter : MonoBehaviour
{
    [SerializeField] public Image fillBar;
    [SerializeField] public TextMeshProUGUI statusText;
    [SerializeField] private Color inRhythmColor = Color.green;
    [SerializeField] private Color offRhythmColor = Color.red;

    public void UpdateBreathFill(float fillPercent)
    {
        // 0 = exhaled, 1 = fully inhaled
        if (fillBar != null)
        {
            fillBar.fillAmount = Mathf.Clamp01(fillPercent);
        }
    }

    public void SetRhythmStatus(bool inRhythm)
    {
        if (statusText != null)
        {
            statusText.text = inRhythm ? "In Rhythm" : "Irregular!";
            statusText.color = inRhythm ? inRhythmColor : offRhythmColor;
        }
    }
}
