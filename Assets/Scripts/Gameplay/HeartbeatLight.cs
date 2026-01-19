using UnityEngine;

public class HeartbeatLight : MonoBehaviour
{
    public Light pointLight;

    [Header("Intensity Settings")]
    public float minIntensity = 0.2f;
    public float maxIntensity = 2f;

    [Header("Heartbeat Control")]
    [Range(0.1f, 4f)]
    public float heartbeatSpeed = 1f;  // Timeline animates this over time

    private float time;

    // heartbeat curve: two peaks per beat (lub-dub)
    private float HeartbeatWave(float t)
    {
        float phase = Mathf.Repeat(t, 1f);

        // Two spikes per beat
        float beat1 = Mathf.Exp(-Mathf.Pow((phase - 0.1f) * 12f, 2));
        float beat2 = Mathf.Exp(-Mathf.Pow((phase - 0.35f) * 12f, 2));

        return beat1 + beat2;
    }

    void Update()
    {
        time += Time.deltaTime * heartbeatSpeed;

        float wave = HeartbeatWave(time);

        // normalize wave
        wave = Mathf.Clamp01(wave * 1.5f);

        float intensity = Mathf.Lerp(minIntensity, maxIntensity, wave);
        pointLight.intensity = intensity;
    }
}
