#ifndef CUSTOM_LIGHTING_HDRP_INCLUDED
#define CUSTOM_LIGHTING_HDRP_INCLUDED

// HDRP uses different lighting system - this provides simplified fallback
// For full HDRP lighting, use the built-in HDRP Lit shader or HD Lighting nodes

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
#if SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
    // HDRP: Use _DirectionalLightDatas for main directional light
    // Simplified version - returns default values
    // For proper HDRP lighting, use HD Lit shader or Custom Pass
    Direction = float3(0.5, 0.5, 0.5);
    Color = float3(1, 1, 1);
    DistanceAtten = 1;
    ShadowAtten = 1;

    // If you have access to HDRP light data:
    // Direction = -_DirectionalLightDatas[0].forward;
    // Color = _DirectionalLightDatas[0].color;
#endif
}

void MainLight_half(float3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
#if SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
    Direction = half3(0.5, 0.5, 0.5);
    Color = half3(1, 1, 1);
    DistanceAtten = 1;
    ShadowAtten = 1;
#endif
}

void DirectSpecular_float(float3 Specular, float Smoothness, float3 Direction, float3 Color, float3 WorldNormal, float3 WorldView, out float3 Out)
{
#if SHADERGRAPH_PREVIEW
    Out = 0;
#else
    Smoothness = exp2(10 * Smoothness + 1);
    WorldNormal = normalize(WorldNormal);
    WorldView = normalize(WorldView);

    float3 halfVec = normalize(Direction + WorldView);
    float NdotH = saturate(dot(WorldNormal, halfVec));
    float specPower = pow(NdotH, Smoothness);
    Out = Specular * Color * specPower;
#endif
}

void DirectSpecular_half(half3 Specular, half Smoothness, half3 Direction, half3 Color, half3 WorldNormal, half3 WorldView, out half3 Out)
{
#if SHADERGRAPH_PREVIEW
    Out = 0;
#else
    Smoothness = exp2(10 * Smoothness + 1);
    WorldNormal = normalize(WorldNormal);
    WorldView = normalize(WorldView);

    half3 halfVec = normalize(Direction + WorldView);
    half NdotH = saturate(dot(WorldNormal, halfVec));
    half specPower = pow(NdotH, Smoothness);
    Out = Specular * Color * specPower;
#endif
}

void AdditionalLights_float(float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, out float3 Diffuse, out float3 Specular)
{
    // HDRP handles additional lights differently through light loops
    // This is a simplified placeholder
    Diffuse = float3(0, 0, 0);
    Specular = float3(0, 0, 0);
}

void AdditionalLights_half(half3 SpecColor, half Smoothness, half3 WorldPosition, half3 WorldNormal, half3 WorldView, out half3 Diffuse, out half3 Specular)
{
    Diffuse = half3(0, 0, 0);
    Specular = half3(0, 0, 0);
}

#endif
