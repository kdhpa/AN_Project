Shader "PostEffect/DitheringHDRP"
{
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

        TEXTURE2D_X(_InputTexture);
        SAMPLER(sampler_InputTexture);

        uint _PatternIndex;
        float _DitherThreshold;
        float _DitherStrength;
        float _DitherScale;

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

        float4x4 GetDitherPattern(uint index)
        {
            float4x4 pattern;

            if (index == 0)
            {
                pattern = float4x4(
                    0, 1, 0, 1,
                    1, 0, 1, 0,
                    0, 1, 0, 1,
                    1, 0, 1, 0
                );
            }
            else if (index == 1)
            {
                pattern = float4x4(
                    0.23, 0.2, 0.6, 0.2,
                    0.2, 0.43, 0.2, 0.77,
                    0.88, 0.2, 0.87, 0.2,
                    0.2, 0.46, 0.2, 0
                );
            }
            else if (index == 2)
            {
                pattern = float4x4(
                    -4.0, 0.0, -3.0, 1.0,
                    2.0, -2.0, 3.0, -1.0,
                    -3.0, 1.0, -4.0, 0.0,
                    3.0, -1.0, 2.0, -2.0
                );
            }
            else if (index == 3)
            {
                pattern = float4x4(
                    1, 0, 0, 1,
                    0, 1, 1, 0,
                    0, 1, 1, 0,
                    1, 0, 0, 1
                );
            }
            else
            {
                pattern = float4x4(
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1
                );
            }

            return pattern;
        }

        float PixelBrightness(float3 col)
        {
            return (col.r + col.g + col.b) / 3.0;
        }

        float Get4x4TexValue(float2 uv, float brightness, float4x4 pattern)
        {
            uint x = (uint)uv.x % 4;
            uint y = (uint)uv.y % 4;

            // Normalize pattern value and compare with brightness
            float threshold = pattern[x][y];
            if (brightness < threshold)
                return 0;
            else
                return 1;
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

            // Dithering
            float2 screenPos = input.positionCS.xy;
            uint2 ditherCoordinate = (uint2)(screenPos / _DitherScale);

            float brightness = PixelBrightness(color.rgb);
            float4x4 ditherPattern = GetDitherPattern(_PatternIndex);
            float ditherPixel = Get4x4TexValue(ditherCoordinate, brightness, ditherPattern);

            float3 result = lerp(color.rgb, color.rgb * ditherPixel, _DitherStrength);
            return float4(result, color.a);
        }
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }

        Pass
        {
            Name "Dithering"
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
