using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/Pixel")]
public sealed class Pixelation : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Pixel width")]
    public ClampedIntParameter pixelWidth = new ClampedIntParameter(64, 1, 512);

    [Tooltip("Pixel height")]
    public ClampedIntParameter pixelHeight = new ClampedIntParameter(64, 1, 512);

    [Tooltip("Resolution")]
    public ClampedIntParameter resolution = new ClampedIntParameter(512, 64, 2048);

    Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    const string kShaderName = "Hidden/Shader/Pixelation";

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            m_Material = new Material(Shader.Find(kShaderName));
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume Pixelation is unable to load.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetFloat("_pixels", resolution.value);
        m_Material.SetFloat("_pw", pixelWidth.value);
        m_Material.SetFloat("_ph", pixelHeight.value);
        m_Material.SetTexture("_MainTex", source);

        HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: 0);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
