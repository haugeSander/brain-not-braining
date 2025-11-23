using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickableReflexButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private float lifetime = 1.5f; // Time before auto-fail
    [SerializeField] private int hrBoostOnClick = 10;
    [SerializeField] private int hrPenaltyOnMiss = 5;

    [Header("Visual References")]
    [SerializeField] private Image timerRing;
    [SerializeField] private ParticleSystem explosionEffect; // Optional

    private float currentTime;
    private bool isClicked = false;
    private ReflexBreathPuzzle puzzleController;

    // Methods to implement:
    // - Start(): Initialize timer, store puzzle reference
    // - Update(): Countdown timer, update ring fill amount
    // - OnPointerClick(): Handle click, trigger green explosion, boost HR
    // - OnTimerExpire(): Trigger red explosion, penalty HR, destroy self

    private void Start()
    {
        currentTime = lifetime;
        puzzleController = FindObjectOfType<ReflexBreathPuzzle>();
    }

    private void Update()
    {
        if (isClicked) return;

        currentTime -= Time.deltaTime;

        // Update timer ring visual
        if (timerRing != null)
        {
            timerRing.fillAmount = currentTime / lifetime;
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
        TriggerExplosion(Color.green);

        // Boost HR
        puzzleController.ModifyHeartRate(hrBoostOnClick);

        // Play success sound (optional)
        // AudioManager.Instance.PlaySFX(clickSound);

        // Destroy self
        Destroy(gameObject);
    }

    private void OnTimerExpire()
    {
        // Trigger red explosion
        TriggerExplosion(Color.red);

        // Penalty HR
        puzzleController.ModifyHeartRate(-hrPenaltyOnMiss);

        // Play fail sound (optional)
        // AudioManager.Instance.PlaySFX(failSound);

        // Destroy self
        Destroy(gameObject);
    }

    private void TriggerExplosion(Color color)
    {
        if (explosionEffect != null)
        {
            var main = explosionEffect.main;
            main.startColor = color;
            explosionEffect.Play();
        }
        // Alternative: Use Animation or simple scale tween
    }
}
