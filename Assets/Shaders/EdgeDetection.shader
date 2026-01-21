Shader "Custom/EdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlendAmount ("Blend Amount", Range(0, 1)) = 0.0
        
        [Space]
        [Header(Brightness)]
        [Toggle] _UseDynamicBrightness ("Use Dynamic Brightness (Pulse/Proximity)?", Float) = 0
        [Space]

        _EdgeThreshold ("Edge Threshold", Range(0, 1)) = 0.1
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _EdgeThickness ("Edge Thickness", Range(1, 3)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "EdgeDetection"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _BlendAmount;
            float _UseDynamicBrightness;
            float _EdgeThreshold;
            float4 _EdgeColor;
            float4 _BackgroundColor;
            float _EdgeThickness;

            // Global properties for proximity-based brightness modulation
            float4 _GlobalEdgeBrightness;  // xyz = player position, w = brightness multiplier
            float _VisibilityRadius;

            // Global properties for echolocation pulse effects
            float4 _PulseData;            // xyz = pulse center position, w = current radius
            float _PulseBrightness;        // Current brightness strength (0-3 range, fades over time)
            float _PulseMaxRadius;         // Maximum radius for falloff calculation

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            // Sobel edge detection using depth
            float SobelDepth(float2 uv, float2 texelSize)
            {
                float thickness = _EdgeThickness;

                // Sample depth in 3x3 grid
                float d00 = SampleSceneDepth(uv + texelSize * float2(-thickness, -thickness));
                float d01 = SampleSceneDepth(uv + texelSize * float2(-thickness, 0));
                float d02 = SampleSceneDepth(uv + texelSize * float2(-thickness, thickness));

                float d10 = SampleSceneDepth(uv + texelSize * float2(0, -thickness));
                float d12 = SampleSceneDepth(uv + texelSize * float2(0, thickness));

                float d20 = SampleSceneDepth(uv + texelSize * float2(thickness, -thickness));
                float d21 = SampleSceneDepth(uv + texelSize * float2(thickness, 0));
                float d22 = SampleSceneDepth(uv + texelSize * float2(thickness, thickness));

                // Sobel kernels
                float sobelX = d00 + 2.0 * d10 + d20 - d02 - 2.0 * d12 - d22;
                float sobelY = d00 + 2.0 * d01 + d02 - d20 - 2.0 * d21 - d22;

                // Combine
                float edge = sqrt(sobelX * sobelX + sobelY * sobelY);
                return edge;
            }

            // Sobel edge detection using normals (better for object edges)
            float SobelNormal(float2 uv, float2 texelSize)
            {
                float thickness = _EdgeThickness;

                // Sample normals in 3x3 grid
                float3 n00 = SampleSceneNormals(uv + texelSize * float2(-thickness, -thickness));
                float3 n01 = SampleSceneNormals(uv + texelSize * float2(-thickness, 0));
                float3 n02 = SampleSceneNormals(uv + texelSize * float2(-thickness, thickness));

                float3 n10 = SampleSceneNormals(uv + texelSize * float2(0, -thickness));
                float3 n12 = SampleSceneNormals(uv + texelSize * float2(0, thickness));

                float3 n20 = SampleSceneNormals(uv + texelSize * float2(thickness, -thickness));
                float3 n21 = SampleSceneNormals(uv + texelSize * float2(thickness, 0));
                float3 n22 = SampleSceneNormals(uv + texelSize * float2(thickness, thickness));

                // Sobel kernels
                float3 sobelX = n00 + 2.0 * n10 + n20 - n02 - 2.0 * n12 - n22;
                float3 sobelY = n00 + 2.0 * n01 + n02 - n20 - 2.0 * n21 - n22;

                // Combine
                float edge = length(sobelX) + length(sobelY);
                return edge;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 texelSize = 1.0 / _ScreenParams.xy;

                // Calculate edges from both depth and normals
                float depthEdge = SobelDepth(input.uv, texelSize);
                float normalEdge = SobelNormal(input.uv, texelSize);

                // Combine edges
                float edge = max(depthEdge * 10.0, normalEdge);

                // Threshold
                edge = step(_EdgeThreshold, edge);

                // Reconstruct world position from depth (needed for both proximity and pulse)
                float depth = SampleSceneDepth(input.uv);
                float3 worldPos = ComputeWorldSpacePosition(input.uv, depth, UNITY_MATRIX_I_VP);

                // Calculate proximity-based brightness
                float proximityBrightness = 0.0;
                if (_VisibilityRadius > 0.0)
                {
                    // Calculate distance from player
                    float3 playerPos = _GlobalEdgeBrightness.xyz;
                    float distance = length(worldPos - playerPos);

                    // Apply distance falloff
                    float normalizedDist = saturate(distance / _VisibilityRadius);
                    proximityBrightness = 1.0 - normalizedDist; // Linear falloff
                    proximityBrightness = proximityBrightness * proximityBrightness; // Quadratic for smoother falloff
                }

                // Calculate pulse-based brightness (expanding sphere effect)
                float pulseBrightness = 0.0;
                if (_PulseBrightness > 0.0 && _PulseMaxRadius > 0.0)
                {
                    float3 pulseCenter = _PulseData.xyz;
                    float currentPulseRadius = _PulseData.w;
                    float distToCenter = length(worldPos - pulseCenter);

                    // Only brighten within current pulse radius (growing sphere)
                    if (distToCenter < currentPulseRadius)
                    {
                        // Smooth falloff from center to edge of pulse sphere
                        float normalizedDist = distToCenter / currentPulseRadius;
                        float falloff = 1.0 - (normalizedDist * normalizedDist); // Quadratic falloff
                        pulseBrightness = _PulseBrightness * falloff;
                    }
                }

                // Conditionally calculate brightness
                float finalBrightness;
                if (_UseDynamicBrightness > 0.5)
                {
                    // Use the original dynamic calculation for pulse and proximity
                    finalBrightness = saturate(proximityBrightness + pulseBrightness);
                }
                else
                {
                    // Use constant full brightness
                    finalBrightness = 1.0f;
                }

                // Apply brightness to edge color
                half4 finalEdgeColor = _EdgeColor * finalBrightness;

                // Output: modulated edges on black background
                half4 sobelColor = lerp(_BackgroundColor, finalEdgeColor, edge);

                // Get original scene color from the texture provided by the Blit pass
                half4 originalSceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                // Blend between original scene and sobel view based on _BlendAmount
                return lerp(originalSceneColor, sobelColor, _BlendAmount);
            }
            ENDHLSL
        }
    }
}
