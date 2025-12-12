using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenEdgeGlowOnHit : MonoBehaviour
{
    [Header("Post Processing")]
    public Volume volume;
    public float flashDuration = 0.3f;
    public float maxIntensity = 0.6f;

    [Header("Collision Settings")]
    public string requiredTag = "Obstacle";
    public float floorNormalThreshold = 0.5f;

    private Vignette vignette;
    private bool isFlashing = false;

    void Start()
    {
        if (volume != null && volume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = 0f;
        }
        else
        {
            Debug.LogWarning($"ScreenEdgeGlowOnHit on {gameObject.name}: Volume or Vignette not found. Edge glow effect will be disabled.");
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Ignore floor
        if (hit.normal.y > floorNormalThreshold) return;

        // Only obstacles
        if (!string.IsNullOrEmpty(requiredTag) &&
            !hit.collider.CompareTag(requiredTag)) return;

        if (!isFlashing && vignette != null)
            StartCoroutine(FlashFadeCoroutine());
    }

    private IEnumerator FlashFadeCoroutine()
    {
        isFlashing = true;

        float t = 0f;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float intensity = Mathf.Lerp(maxIntensity, 0f, t / flashDuration);
            vignette.intensity.value = intensity;
            yield return null;
        }

        vignette.intensity.value = 0f;
        isFlashing = false;
    }
}
