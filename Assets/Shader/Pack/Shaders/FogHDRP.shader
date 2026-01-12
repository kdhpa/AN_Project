Shader "PostEffect/FogHDRP"
{
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

        TEXTURE2D_X(_InputTexture);
        SAMPLER(sampler_InputTexture);

        float _FogDensity;
        float _FogDistance;
        float4 _PSXFogColor;
        float _FogNear;
        float _FogFar;
        float _FogAltScale;
        float _FogThinning;
        float _NoiseScale;
        float _NoiseStrength;

        struct Attributes
        {
            uint vertexID : SV_VertexID;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 texcoord : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        // Simple noise function
        float hash(float2 p)
        {
            return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
        }

        float noise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            f = f * f * (3.0 - 2.0 * f);

            float a = hash(i);
            float b = hash(i + float2(1.0, 0.0));
            float c = hash(i + float2(0.0, 1.0));
            float d = hash(i + float2(1.0, 1.0));

            return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
        }

        float ComputeDistance(float depth)
        {
            float dist = depth * _ProjectionParams.z;
            dist -= _ProjectionParams.y * _FogDistance;
            return dist;
        }

        float ComputeFog(float z, float density)
        {
            float fog = exp2(density * z);
            return saturate(fog);
        }

        Varyings Vert(Attributes input)
        {
            Varyings output;
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
            output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
            return output;
        }

        float4 Frag(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            float2 uv = input.texcoord;
            float4 color = SAMPLE_TEXTURE2D_X(_InputTexture, sampler_InputTexture, uv);

            // Depth
            float depth = LoadCameraDepth(uv * _ScreenSize.xy);
            float linearDepth = Linear01Depth(depth, _ZBufferParams);

            // Fog calculation
            float dist = ComputeDistance(linearDepth);
            float fog = 1.0 - ComputeFog(dist, _FogDensity);

            // Noise
            float2 screenPos = input.positionCS.xy;
            float screenNoise = noise(screenPos / _NoiseScale);

            // Ambient
            float4 ambientColor = float4(0.1, 0.1, 0.1, 0.1);

            // Final blend
            float fogFactor = saturate(fog + (screenNoise * _NoiseStrength));
            float4 result = lerp(color, _PSXFogColor * ambientColor, fogFactor);

            return result;
        }
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }

        Pass
        {
            Name "Fog"
            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
                #pragma target 4.5
            ENDHLSL
        }
    }

    Fallback Off
}
