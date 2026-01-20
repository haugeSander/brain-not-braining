Shader "Custom/BarycentricWireframe"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0, 0, 0, 0)
        _WireColor ("Wire Color", Color) = (1, 0.5, 0, 1) // Default Orange
        _WireThickness ("Wire Thickness", Range(0, 0.8)) = 0.05
        _CenterFade ("Center Fade", Range(0, 1)) = 0.5 // Higher = Fades center lines more
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On  // Changed to ON to help with depth sorting
        Cull Back  // CHANGED: Hides the back of the brain to reduce visual noise

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL; // We need normals for 3D effect
                float3 uv2 : TEXCOORD1; 
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD3; // World Space position
                float3 normalWS : TEXCOORD4;   // World Space normal
                float3 barycentric : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _WireColor;
                float _WireThickness;
                float _CenterFade;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.barycentric = input.uv2;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 1. Calculate Wireframe Logic
                float3 barys = input.barycentric;
                float minBary = min(barys.x, min(barys.y, barys.z));
                float delta = fwidth(minBary);
                float wireAlpha = 1.0 - smoothstep(_WireThickness, _WireThickness + delta, minBary);

                // 2. Calculate 3D "Rim" Effect (Fresnel)
                // This makes lines facing the camera dimmer, and edge lines brighter
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);
                float3 normal = normalize(input.normalWS);
                float NdotV = abs(dot(normal, viewDir));
                
                // Fade out wires that are facing directly at the camera
                float depthFactor = 1.0 - (NdotV * _CenterFade);
                
                // Apply colors
                half4 finalColor = _WireColor;
                finalColor.a = wireAlpha * depthFactor;

                // Combine with base transparency
                return finalColor + _BaseColor * (1.0 - finalColor.a);
            }
            ENDHLSL
        }
    }
}