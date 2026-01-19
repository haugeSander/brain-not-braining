using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// URP Renderer Feature for BLIND VR-style edge detection.
/// Applies Sobel edge detection to create outline-only, monochrome "echo-vision" rendering.
/// Compatible with URP 14+ (Unity 2022+) using modern RTHandle API.
/// </summary>
public class EdgeDetectionFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class EdgeDetectionSettings
    {
        [Header("Edge Detection")]
        [Tooltip("Material with EdgeDetection shader")]
        public Material edgeDetectionMaterial;

        [Tooltip("Edge detection sensitivity (0-1)")]
        [Range(0f, 1f)]
        public float edgeThreshold = 0.1f;

        [Tooltip("Edge line thickness (1-3)")]
        [Range(1f, 3f)]
        public float edgeThickness = 1f;

        [Header("Colors")]
        [Tooltip("Color for detected edges")]
        public Color edgeColor = Color.white;

        [Tooltip("Background color (usually black)")]
        public Color backgroundColor = Color.black;

        [Header("Render Settings")]
        [Tooltip("When to inject this render pass")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public EdgeDetectionSettings settings = new EdgeDetectionSettings();
    private EdgeDetectionPass edgeDetectionPass;

    public override void Create()
    {
        edgeDetectionPass = new EdgeDetectionPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.edgeDetectionMaterial == null)
        {
            Debug.LogWarning("EdgeDetectionFeature: Material is not assigned!");
            return;
        }

        // Pass the renderer to the pass so it can access cameraColorTargetHandle internally
        edgeDetectionPass.SetupRenderer(renderer);
        renderer.EnqueuePass(edgeDetectionPass);
    }

    protected override void Dispose(bool disposing)
    {
        edgeDetectionPass?.Dispose();
    }

    class EdgeDetectionPass : ScriptableRenderPass
    {
        private EdgeDetectionSettings settings;
        private ScriptableRenderer renderer;
        private RTHandle tempTextureHandle;

        private static readonly int EdgeThresholdID = Shader.PropertyToID("_EdgeThreshold");
        private static readonly int EdgeColorID = Shader.PropertyToID("_EdgeColor");
        private static readonly int BackgroundColorID = Shader.PropertyToID("_BackgroundColor");
        private static readonly int EdgeThicknessID = Shader.PropertyToID("_EdgeThickness");
        private static readonly int GlobalEdgeColorID = Shader.PropertyToID("_GlobalEdgeColor");

        public EdgeDetectionPass(EdgeDetectionSettings settings)
        {
            this.settings = settings;
            this.renderPassEvent = settings.renderPassEvent;
        }

        public void SetupRenderer(ScriptableRenderer renderer)
        {
            this.renderer = renderer;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Get camera descriptor
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0; // No depth needed for post-process
            descriptor.msaaSamples = 1; // No MSAA for post-process

            // Allocate or reallocate temporary RTHandle
            RenderingUtils.ReAllocateIfNeeded(
                ref tempTextureHandle,
                descriptor,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_EdgeDetectionTemp"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.edgeDetectionMaterial == null || renderer == null)
                return;

            // Check if temp texture is allocated
            if (tempTextureHandle == null)
            {
                Debug.LogWarning("EdgeDetection: Temporary texture is null!");
                return;
            }

            // Access camera color target inside the render pass (correct scope)
            RTHandle cameraColorTarget = renderer.cameraColorTargetHandle;

            if (cameraColorTarget == null)
            {
                Debug.LogWarning("EdgeDetection: Camera color target is null!");
                return;
            }

            // Additional validation - check if RTHandles have valid RenderTextures
            if (tempTextureHandle.rt == null && tempTextureHandle.nameID == 0)
            {
                Debug.LogWarning("EdgeDetection: Temp RTHandle is not properly initialized!");
                return;
            }

            if (cameraColorTarget.rt == null && cameraColorTarget.nameID == 0)
            {
                Debug.LogWarning("EdgeDetection: Camera RTHandle is not properly initialized!");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("EdgeDetection");

            // Set shader properties
            settings.edgeDetectionMaterial.SetFloat(EdgeThresholdID, settings.edgeThreshold);
            settings.edgeDetectionMaterial.SetColor(EdgeColorID, settings.edgeColor);
            settings.edgeDetectionMaterial.SetColor(BackgroundColorID, settings.backgroundColor);
            settings.edgeDetectionMaterial.SetFloat(EdgeThicknessID, settings.edgeThickness);

            try
            {
                // Use legacy Blit for Unity 6 Compatibility Mode
                // Explicitly convert RTHandles to RenderTargetIdentifier for compatibility
                RenderTargetIdentifier sourceId = new RenderTargetIdentifier(cameraColorTarget);
                RenderTargetIdentifier tempId = new RenderTargetIdentifier(tempTextureHandle);

                // Step 1: Blit from camera color to temp with edge detection shader
                cmd.Blit(sourceId, tempId, settings.edgeDetectionMaterial, 0);

                // Step 2: Blit from temp back to camera color (no shader, just copy)
                cmd.Blit(tempId, sourceId);

                context.ExecuteCommandBuffer(cmd);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"EdgeDetection: Blit failed - {e.Message}");
            }
            finally
            {
                CommandBufferPool.Release(cmd);
            }
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Cleanup happens in Dispose
        }

        public void Dispose()
        {
            tempTextureHandle?.Release();
        }
    }
}
