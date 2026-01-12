Shader "PostEffect/PixelationHDRP"
{
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

        TEXTURE2D_X(_InputTexture);
        SAMPLER(sampler_InputTexture);

        float _WidthPixelation;
        float _HeightPixelation;
        float _ColorPrecision;

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

            // Pixelation
            uv.x = floor(uv.x * _WidthPixelation) / _WidthPixelation;
            uv.y = floor(uv.y * _HeightPixelation) / _HeightPixelation;

            float4 color = SAMPLE_TEXTURE2D_X(_InputTexture, sampler_InputTexture, uv);

            // Color precision
            color = floor(color * _ColorPrecision) / _ColorPrecision;

            return color;
        }
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }

        Pass
        {
            Name "Pixelation"
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
