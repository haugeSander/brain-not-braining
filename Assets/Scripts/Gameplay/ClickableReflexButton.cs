using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BrainNotBraining.Core;
using System.Collections;

public class ClickableReflexButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private float lifetime = 1.5f; // Time before auto-fail
    [SerializeField] private int hrBoostOnClick = 10;
    [SerializeField] private int hrPenaltyOnMiss = 10; // Penalty for missing button

    [Header("Visual References")]
    [SerializeField] private Image timerRing;
    [SerializeField] private ParticleSystem explosionEffect; // Optional

    [Header("Animation")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.1f;
    [SerializeField] private float urgencyThreshold = 0.5f; // Start urgent pulse when < 0.5s left
    [SerializeField] private AnimationCurve urgencyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Timer Ring Colors")]
    [SerializeField] private Color ringColorFull = Color.white; // When time is full
    [SerializeField] private Color ringColorHalf = Color.yellow; // When time is half
    [SerializeField] private Color ringColorLow = Color.red; // When time is low

    private float currentTime;
    private bool isClicked = false;
    private bool hasExpired = false; // Prevent multiple OnTimerExpire calls
    private ReflexBreathPuzzle puzzleController;
    private RectTransform rectTransform;
    private Vector3 baseScale;

    // Methods to implement:
    // - Start(): Initialize timer, store puzzle reference
    // - Update(): Countdown timer, update ring fill amount
    // - OnPointerClick(): Handle click, trigger green explosion, boost HR
    // - OnTimerExpire(): Trigger red explosion, penalty HR, destroy self

    private void Start()
    {
        currentTime = lifetime;
        puzzleController = FindObjectOfType<ReflexBreathPuzzle>();
        rectTransform = GetComponent<RectTransform>();
        baseScale = rectTransform.localScale;

        // Scale up from 0
        rectTransform.localScale = Vector3.zero;
        StartCoroutine(SpawnAnimation());
    }

    private IEnumerator SpawnAnimation()
    {
        float elapsed = 0f;
        float duration = 0.2f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rectTransform.localScale = Vector3.one * Mathf.Lerp(0, 1, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localScale = baseScale;
    }


    private void Update()
    {
        if (isClicked || hasExpired) return; // Stop updating if clicked or expired

        currentTime -= Time.deltaTime;

        // Calculate time percentage
        float timePercent = currentTime / lifetime;

        // Update timer ring visual
        if (timerRing != null)
        {
            timerRing.fillAmount = timePercent;

            // Change color based on remaining time
            // White → Yellow → Red
            if (timePercent > 0.5f)
            {
                // Interpolate between white and yellow (100% to 50%)
                float t = (timePercent - 0.5f) / 0.5f; // 0 at 50%, 1 at 100%
                timerRing.color = Color.Lerp(ringColorHalf, ringColorFull, t);
            }
            else
            {
                // Interpolate between yellow and red (50% to 0%)
                float t = timePercent / 0.5f; // 0 at 0%, 1 at 50%
                timerRing.color = Color.Lerp(ringColorLow, ringColorHalf, t);
            }
        }

        // Pulse animation - only when time is running out
        if (currentTime < urgencyThreshold)
        {
            // Calculate urgency (0 to 1, where 1 is most urgent)
            float urgency = 1f - (currentTime / urgencyThreshold);
            float pulse = Mathf.Sin(Time.time * pulseSpeed * (1f + urgency * 5f)) * pulseAmount * (1f + urgency);
            rectTransform.localScale = baseScale * (1f + pulse);
        }
        else
        {
            // Subtle idle pulse when plenty of time left
            float pulse = Mathf.Sin(Time.time * pulseSpeed * 0.5f) * (pulseAmount * 0.3f);
            rectTransform.localScale = baseScale * (1f + pulse);
        }

        // Check if time expired
        if (currentTime <= 0)
        {
            OnTimerExpire();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked) return;

        isClicked = true;

        // Trigger green explosion
        StartCoroutine(ExplosionEffect(Color.green));

        // Boost HR
        puzzleController.ModifyHeartRate(hrBoostOnClick);

        // Play success sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // Destroy self after explosion
        Destroy(gameObject, 0.3f);
    }

    private void OnTimerExpire()
    {
        if (hasExpired) return; // Already expired, don't trigger again
        hasExpired = true;

        Debug.Log($"Button missed! Applying -{hrPenaltyOnMiss} HR penalty");

        // Trigger red explosion
        StartCoroutine(ExplosionEffect(Color.red));

        // Penalty HR
        if (puzzleController != null)
        {
            puzzleController.ModifyHeartRate(-hrPenaltyOnMiss);
        }
        else
        {
            Debug.LogError("PuzzleController is null! Cannot apply HR penalty.");
        }

        // Play fail sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonMiss();
        }

        // Destroy self after explosion
        Destroy(gameObject, 0.3f);
    }

    private IEnumerator ExplosionEffect(Color color)
    {
        // Create reliable UI-based explosion
        Transform canvasTransform = GetComponentInParent<Canvas>().transform;
        UIExplosion.Create(rectTransform.anchoredPosition, color, canvasTransform);

        Debug.Log($"UI Explosion created at position: {rectTransform.anchoredPosition}, Color: {color}");

        // Also trigger particle system if available (as bonus effect)
        if (explosionEffect != null)
        {
            // Get button's WORLD position before any changes
            Vector3 buttonWorldPosition = rectTransform.position;

            Debug.Log($"Explosion triggered! Color: {color}, Button world position: {buttonWorldPosition}");

            // Configure particle system for 2D Canvas rendering
            var main = explosionEffect.main;
            main.startColor = color;
            main.simulationSpace = ParticleSystemSimulationSpace.World; // Use World space for canvas compatibility

            // Configure renderer to show above canvas UI
            ParticleSystemRenderer renderer = explosionEffect.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "Default";
                renderer.sortingOrder = 1000; // Render above everything
                renderer.renderMode = ParticleSystemRenderMode.Billboard; // Face camera
            }

            // Reparent to Canvas, preserving world position
            explosionEffect.transform.SetParent(canvasTransform, worldPositionStays: true);

            // Adjust Z to be in front of camera (for visibility)
            Vector3 explosionPos = explosionEffect.transform.position;
            explosionPos.z = Camera.main != null ? Camera.main.transform.position.z + 5f : 5f;
            explosionEffect.transform.position = explosionPos;

            // Ensure particle system is active and play
            explosionEffect.gameObject.SetActive(true);
            explosionEffect.Play();

            Debug.Log($"Particle explosion started at world position: {explosionEffect.transform.position}");

            // Destroy particle system after it's done
            float totalDuration = main.duration + main.startLifetime.constantMax;
            Destroy(explosionEffect.gameObject, totalDuration);
        }
        else
        {
            Debug.LogWarning("ExplosionEffect particle system is null! Assign in Inspector.");
        }

        // Simple fade out effect (no scaling/square expansion)
        Image buttonImage = GetComponent<Image>();
        Image timerRingImage = timerRing;

        float duration = 0.3f;
        float elapsed = 0f;
        Color originalButtonColor = buttonImage != null ? buttonImage.color : Color.white;
        Color originalRingColor = timerRingImage != null ? timerRingImage.color : Color.white;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float alpha = 1f - t; // Fade out

            // Fade out both button and ring
            if (buttonImage != null)
            {
                Color c = originalButtonColor;
                c.a = alpha;
                buttonImage.color = c;
            }

            if (timerRingImage != null)
            {
                Color c = originalRingColor;
                c.a = alpha;
                timerRingImage.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
