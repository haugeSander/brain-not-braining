using UnityEngine;
using UnityEngine.UI; // Required for Slider
using TMPro; // Required for TextMeshPro

/// <summary>
/// Manages all UI elements for Level 4, such as the sight meter and pickup counter.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The slider representing the remaining time for the sight ability.")]
    public Slider sightMeterSlider;

    [Tooltip("The text that displays how many pickups have been collected.")]
    public TextMeshProUGUI pickupCounterText;

    /// <summary>
    /// Updates the sight meter slider.
    /// </summary>
    /// <param name="currentValue">The current sight time.</param>
    /// <param name="maxValue">The maximum sight time.</param>
    public void UpdateSightMeter(float currentValue, float maxValue)
    {
        if (sightMeterSlider != null)
        {
            sightMeterSlider.maxValue = maxValue;
            sightMeterSlider.value = currentValue;
        }
    }

    /// <summary>
    /// Updates the pickup counter text.
    /// </summary>
    /// <param name="currentCount">How many pickups have been collected.</param>
    /// <param name="totalCount">How many pickups exist in the level.</param>
    public void UpdatePickupCount(int currentCount, int totalCount)
    {
        if (pickupCounterText != null)
        {
            pickupCounterText.text = $"OBJECTS: {currentCount} / {totalCount}";
        }
    }
}
