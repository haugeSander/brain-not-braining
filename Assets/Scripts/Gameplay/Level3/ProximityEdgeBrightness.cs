using UnityEngine;
using BrainNotBraining.Core;
using BrainNotBraining.Gameplay;

/// <summary>
/// Modulates edge detection brightness based on proximity to player.
/// Creates an "expanding glow" effect where edges brighten as player approaches.
/// Works with EdgeDetection shader by adjusting global edge color intensity.
///
/// INTEGRATION NOTE: Works alongside EcholocationPulseManager.
/// - This system provides continuous base proximity brightness.
/// - Pulse system adds temporary brightness boosts on top.
/// - Shader combines both additively: finalBrightness = proximityBrightness + pulseBrightness.
/// </summary>
public class ProximityEdgeBrightness : MonoBehaviour
{
    [Header("Proximity Glow Configuration")]
    [Tooltip("Maximum distance for proximity glow effect")]
    public float glowRadius = 5f;

    [Tooltip("Base edge brightness (0-1)")]
    [Range(0f, 2f)]
    public float baseEdgeBrightness = 0.8f;

    [Tooltip("Maximum edge brightness when very close (0-1)")]
    [Range(0f, 3f)]
    public float maxEdgeBrightness = 2.0f;

    [Tooltip("Falloff curve (higher = sharper falloff)")]
    [Range(1f, 4f)]
    public float falloffExponent = 2f;

    [Header("Update Configuration")]
    [Tooltip("Update frequency in Hz")]
    [Range(5f, 30f)]
    public float updateFrequency = 15f;

    [Header("Layer Configuration")]
    [Tooltip("Layers to check for proximity")]
    public LayerMask proximityLayerMask = ~0;

    private float updateInterval;
    private float nextUpdateTime;
    private static readonly int GlobalEdgeBrightnessID = Shader.PropertyToID("_GlobalEdgeBrightness");
    private static readonly int VisibilityRadiusID = Shader.PropertyToID("_VisibilityRadius");

    private void Awake()
    {
        updateInterval = 1f / updateFrequency;
        nextUpdateTime = Time.time;
    }

    private void Update()
    {
        // Throttled update
        if (Time.time < nextUpdateTime)
        {
            return;
        }

        nextUpdateTime = Time.time + updateInterval;

        UpdateProximityGlow();
    }

    private void UpdateProximityGlow()
    {
        // Pass player position and visibility radius to shader
        Vector3 playerPos = transform.position;
        Vector4 edgeBrightnessData = new Vector4(playerPos.x, playerPos.y, playerPos.z, 1.0f);

        Shader.SetGlobalVector(GlobalEdgeBrightnessID, edgeBrightnessData);
        Shader.SetGlobalFloat(VisibilityRadiusID, glowRadius);
    }

    private void OnDisable()
    {
        // Disable visibility radius when component is disabled
        Shader.SetGlobalFloat(VisibilityRadiusID, 0f);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw glow radius
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, glowRadius);
    }
    #endif
}
