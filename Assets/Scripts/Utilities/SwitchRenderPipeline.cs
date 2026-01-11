using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Switches the active Render Pipeline Asset for a specific scene.
/// Useful for applying different visual styles to different levels.
/// Note: This changes the PROJECT-LEVEL setting, not per-scene.
/// Recommended approach: Manually toggle EdgeDetectionFeature on/off instead.
/// </summary>
public class SwitchRenderPipeline : MonoBehaviour
{
    [Header("Render Pipeline Override")]
    [Tooltip("Render Pipeline Asset to use for this scene (e.g., Level 3 edge detection)")]
    public RenderPipelineAsset sceneRenderPipelineAsset;

    [Tooltip("Automatically revert to original pipeline on scene unload")]
    public bool revertOnSceneUnload = true;

    private RenderPipelineAsset originalPipeline;

    private void Awake()
    {
        if (sceneRenderPipelineAsset == null)
        {
            Debug.LogWarning("SwitchRenderPipeline: No Render Pipeline Asset assigned!");
            return;
        }

        // Store original pipeline
        originalPipeline = QualitySettings.renderPipeline;

        // Switch to scene-specific pipeline (project-level setting)
        QualitySettings.renderPipeline = sceneRenderPipelineAsset;

        Debug.Log($"Switched to render pipeline: {sceneRenderPipelineAsset.name}");
    }

    private void OnDestroy()
    {
        // Revert to original pipeline when scene unloads
        if (revertOnSceneUnload && originalPipeline != null)
        {
            QualitySettings.renderPipeline = originalPipeline;
            Debug.Log($"Reverted to render pipeline: {originalPipeline.name}");
        }
    }
}
