using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class FullscreenBlitFeature : ScriptableRendererFeature
{
    class BlitPass : ScriptableRenderPass
    {
        private class PassData
        {
            internal TextureHandle src;
            internal TextureHandle dst;
            internal Material blitMaterial;
        }

        readonly Material blitMaterial;

        public BlitPass(Material material)
        {
            blitMaterial = material;
            //renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            // in BlitPass ctor
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "Fullscreen Blit After Transparents";
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                passData.src = resourceData.activeColorTexture;
                passData.dst = resourceData.activeColorTexture;
                passData.blitMaterial = blitMaterial;

                builder.UseTexture(passData.src);
                builder.UseTexture(passData.dst, 0);

                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    Blitter.BlitTexture(ctx.cmd, data.src, Vector4.one, data.blitMaterial, 0);
                });
            }
        }
    }

    [Tooltip("Material with your full-screen shader")]
    public Material blitMaterial;

    BlitPass blitPass;

    public override void Create()
    {
        if (blitMaterial == null)
            Debug.LogError("Assign a material with your fullscreen shader on \"" + name + "\".");
        blitPass = new BlitPass(blitMaterial);
    }

    // Enqueue the pass every frame
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(blitPass);
    }
}
