using UnityEngine;
using System.Collections;

public class LightUpOnCollisionLevel1 : MonoBehaviour

{
    [Header("Light Settings")]
    public Light light1;
    public Light light2;
    public float flashDuration = 1f; // duration of fade
    public float maxIntensity = 5f;  // maximum light intensity

    [Header("Collision Settings")]
    public string requiredTag = "Obstacle";  // only objects with this tag trigger the flash
    public float floorNormalThreshold = 0.5f; // ignore collisions with floor

    private bool isFlashing = false;

    private void Start()
    {
        if (light1 != null)
        {
            light1.enabled = false;
            light1.intensity = maxIntensity;
        }

        if (light2 != null)
        {
            light2.enabled = false;
            light2.intensity = maxIntensity;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Ignore floor
        //if (hit.normal.y > floorNormalThreshold) return;

        // Only objects with the required tag trigger
        if (!string.IsNullOrEmpty(requiredTag) && !hit.collider.CompareTag(requiredTag)) return;

        if (!isFlashing)
            StartCoroutine(FlashFadeCoroutine());
    }

    private IEnumerator FlashFadeCoroutine()
    {
        isFlashing = true;

        if (light1 != null) light1.enabled = true;
        if (light2 != null) light2.enabled = true;

        float t = 0f;
        float startIntensity = maxIntensity;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float currentIntensity = Mathf.Lerp(startIntensity, 0f, t / flashDuration);

            if (light1 != null) light1.intensity = currentIntensity;
            if (light2 != null) light2.intensity = currentIntensity;

            yield return null;
        }

        if (light1 != null) light1.intensity = maxIntensity;
        if (light1 != null) light1.enabled = false;

        if (light2 != null) light2.intensity = maxIntensity;
        if (light2 != null) light2.enabled = false;

        isFlashing = false;
    }
}

/*
{
    [Header("Light Settings")]
    public Light lightSource;
    public float flashDuration = 1f; // Dauer des Abklingens
    public float maxIntensity = 5f;  // maximale Lichtstärke

    [Header("Collision Settings")]
    public string requiredTag = "Obstacle";  // nur Objekte mit diesem Tag triggern das Licht
    public float floorNormalThreshold = 0.5f; // alles mit normal.y > threshold wird als Boden ignoriert

    private bool isFlashing = false;

    private void Start()
    {
        if (lightSource != null)
        {
            lightSource.enabled = false;
            lightSource.intensity = maxIntensity;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Boden ignorieren
        if (hit.normal.y > floorNormalThreshold) return;

        // Nur Objekte mit dem richtigen Tag triggern das Licht
        if (!string.IsNullOrEmpty(requiredTag) && !hit.collider.CompareTag(requiredTag)) return;

        if (!isFlashing)
            StartCoroutine(FlashFadeCoroutine());
    }

    private IEnumerator FlashFadeCoroutine()
    {
        isFlashing = true;
        lightSource.enabled = true;

        float t = 0f;
        float startIntensity = maxIntensity;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            lightSource.intensity = Mathf.Lerp(startIntensity, 0f, t / flashDuration);
            yield return null;
        }

        lightSource.intensity = maxIntensity; // reset für nächsten Flash
        lightSource.enabled = false;
        isFlashing = false;
    }
}
*/





/*
{
    public Light lightSource;
    public float flashDuration = 1f;
    public string requiredTag = "Obstacle";

    private bool isFlashing = false;

    private void Start()
    {
        if (lightSource != null)
            lightSource.enabled = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Only react to objects with the required tag
        if (!hit.collider.CompareTag(requiredTag)) 
            return;

        // Prevent repeated triggers
        if (!isFlashing)
            StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        isFlashing = true;
        lightSource.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        lightSource.enabled = false;
        isFlashing = false;
    }
}
*/