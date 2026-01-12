using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace PSX
{
    [System.Serializable, VolumeComponentMenu("Post-processing/PSX/Fog")]
    public class FogEffect : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        // Parameters
        public ClampedFloatParameter fogDensity = new ClampedFloatParameter(0f, 0f, 10f);
        public ClampedFloatParameter fogDistance = new ClampedFloatParameter(10f, 0f, 100f);
        public ColorParameter fogColor = new ColorParameter(Color.white);
        public ClampedFloatParameter fogNear = new ClampedFloatParameter(1f, 0f, 100f);
        public ClampedFloatParameter fogFar = new ClampedFloatParameter(100f, 0f, 100f);
        public ClampedFloatParameter fogAltScale = new ClampedFloatParameter(10f, 0f, 100f);
        public ClampedFloatParameter fogThinning = new ClampedFloatParameter(100f, 0f, 1000f);
        public ClampedFloatParameter noiseScale = new ClampedFloatParameter(100f, 0f, 1000f);
        public ClampedFloatParameter noiseStrength = new ClampedFloatParameter(0.05f, 0f, 1f);

        Material material;

        static readonly int InputTextureId = Shader.PropertyToID("_InputTexture");
        static readonly int FogDensityId = Shader.PropertyToID("_FogDensity");
        static readonly int FogDistanceId = Shader.PropertyToID("_FogDistance");
        static readonly int FogColorId = Shader.PropertyToID("_PSXFogColor");
        static readonly int FogNearId = Shader.PropertyToID("_FogNear");
        static readonly int FogFarId = Shader.PropertyToID("_FogFar");
        static readonly int FogAltScaleId = Shader.PropertyToID("_FogAltScale");
        static readonly int FogThinningId = Shader.PropertyToID("_FogThinning");
        static readonly int NoiseScaleId = Shader.PropertyToID("_NoiseScale");
        static readonly int NoiseStrengthId = Shader.PropertyToID("_NoiseStrength");

        public bool IsActive() => fogDensity.value > 0f && material != null;

        public override CustomPostProcessInjectionPoint injectionPoint =>
            CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            var shader = Shader.Find("PostEffect/FogHDRP");
            if (shader != null)
            {
                material = CoreUtils.CreateEngineMaterial(shader);
            }
            else
            {
                Debug.LogError("Fog shader not found at 'PostEffect/FogHDRP'");
            }
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (material == null)
                return;

            material.SetTexture(InputTextureId, source);
            material.SetFloat(FogDensityId, fogDensity.value);
            material.SetFloat(FogDistanceId, fogDistance.value);
            material.SetColor(FogColorId, fogColor.value);
            material.SetFloat(FogNearId, fogNear.value);
            material.SetFloat(FogFarId, fogFar.value);
            material.SetFloat(FogAltScaleId, fogAltScale.value);
            material.SetFloat(FogThinningId, fogThinning.value);
            material.SetFloat(NoiseScaleId, noiseScale.value);
            material.SetFloat(NoiseStrengthId, noiseStrength.value);

            cmd.Blit(source, destination, material, 0);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(material);
        }
    }
}
