using UnityEngine;

/// <summary>
/// Controls the Edge Detection shader blend amount globally.
/// This avoids switching the entire Render Pipeline Asset.
/// </summary>
public class ToggleEdgeEffect : MonoBehaviour
{
    [Header("Settings")]
    [Range(0, 1)]
    [SerializeField] private float activeBlendValue = 1.0f;
    [SerializeField] private float inactiveBlendValue = 0.0f;

    [Tooltip("If true, the effect will revert to 0 when this object is destroyed/scene changed.")]
    [SerializeField] private bool revertOnDisable = true;

    // This matches the variable name in your HLSL code
    private static readonly int BlendAmountProp = Shader.PropertyToID("_BlendAmount");

    private void Start()
    {
        // Set the blend to 1 (or your active value) when the level starts
        Shader.SetGlobalFloat(BlendAmountProp, activeBlendValue);
        Debug.Log("Edge Detection: Enabled");
    }

    private void OnDisable()
    {
        // Set the blend back to 0 when the level ends or the object is removed
        if (revertOnDisable)
        {
            Shader.SetGlobalFloat(BlendAmountProp, inactiveBlendValue);
            Debug.Log("Edge Detection: Disabled");
        }
    }
}