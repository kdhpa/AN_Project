using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace PSX
{
    [System.Serializable, VolumeComponentMenu("Post-processing/PSX/Pixelation")]
    public class PixelationEffect : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        // Parameters
        public ClampedFloatParameter widthPixelation = new ClampedFloatParameter(512f, 1f, 1920f);
        public ClampedFloatParameter heightPixelation = new ClampedFloatParameter(512f, 1f, 1080f);
        public ClampedFloatParameter colorPrecision = new ClampedFloatParameter(32f, 1f, 256f);

        Material material;

        static readonly int InputTextureId = Shader.PropertyToID("_InputTexture");
        static readonly int WidthPixelationId = Shader.PropertyToID("_WidthPixelation");
        static readonly int HeightPixelationId = Shader.PropertyToID("_HeightPixelation");
        static readonly int ColorPrecisionId = Shader.PropertyToID("_ColorPrecision");

        public bool IsActive() => material != null;

        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            var shader = Shader.Find("PostEffect/PixelationHDRP");
            if (shader != null)
            {
                material = CoreUtils.CreateEngineMaterial(shader);
            }
            else
            {
                Debug.LogError("Pixelation shader not found at 'PostEffect/PixelationHDRP'");
            }
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (material == null)
                return;

            material.SetTexture(InputTextureId, source);

            // Use fixed pixelation regardless of resolution
            material.SetFloat(WidthPixelationId, widthPixelation.value);
            material.SetFloat(HeightPixelationId, heightPixelation.value);
            material.SetFloat(ColorPrecisionId, colorPrecision.value);

            cmd.Blit(source, destination, material, 0);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(material);
        }
    }
}
