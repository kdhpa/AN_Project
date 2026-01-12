using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace PSX
{
    [System.Serializable, VolumeComponentMenu("Post-processing/PSX/Dithering")]
    public class DitheringEffect : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        // Parameters
        public ClampedIntParameter patternIndex = new ClampedIntParameter(0, 0, 10);
        public ClampedFloatParameter ditherThreshold = new ClampedFloatParameter(512f, 0f, 1024f);
        public ClampedFloatParameter ditherStrength = new ClampedFloatParameter(1f, 0f, 2f);
        public ClampedFloatParameter ditherScale = new ClampedFloatParameter(2f, 0.1f, 10f);

        Material material;

        static readonly int InputTextureId = Shader.PropertyToID("_InputTexture");
        static readonly int PatternIndexId = Shader.PropertyToID("_PatternIndex");
        static readonly int DitherThresholdId = Shader.PropertyToID("_DitherThreshold");
        static readonly int DitherStrengthId = Shader.PropertyToID("_DitherStrength");
        static readonly int DitherScaleId = Shader.PropertyToID("_DitherScale");

        public bool IsActive() => material != null && ditherStrength.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            var shader = Shader.Find("PostEffect/DitheringHDRP");
            if (shader != null)
            {
                material = CoreUtils.CreateEngineMaterial(shader);
            }
            else
            {
                Debug.LogError("Dithering shader not found at 'PostEffect/DitheringHDRP'");
            }
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (material == null)
                return;

            material.SetTexture(InputTextureId, source);
            material.SetInt(PatternIndexId, patternIndex.value);
            material.SetFloat(DitherThresholdId, ditherThreshold.value);
            material.SetFloat(DitherStrengthId, ditherStrength.value);
            material.SetFloat(DitherScaleId, ditherScale.value);

            cmd.Blit(source, destination, material, 0);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(material);
        }
    }
}
