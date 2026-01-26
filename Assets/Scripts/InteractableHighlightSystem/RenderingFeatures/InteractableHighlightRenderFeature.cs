using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.Universal;

public class InteractableHighlightRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingOpaques;
        public LayerMask layerMask = ~0;
        public uint renderingLayerMask = 1u;
        public RenderQueueMode renderQueue = RenderQueueMode.Opaque;
        public Material overrideMaterial;
        public int overrideMaterialPassIndex = 0;
        public bool overrideDepthState = false;
        public CompareFunction depthCompare = CompareFunction.LessEqual;
        public bool depthWrite = false;

        public enum RenderQueueMode
        {
            Opaque,
            Transparent,
            All
        }
    }

    public Settings settings = new Settings();

    private HighlightPass _pass;

    public override void Create()
    {
        _pass = new HighlightPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.overrideMaterial == null)
            return;

        _pass.UpdateSettings(settings);
        renderer.EnqueuePass(_pass);
    }

    private class HighlightPass : ScriptableRenderPass
    {
        private static readonly List<ShaderTagId> ShaderTags = new List<ShaderTagId>
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit")
        };

        private Settings _settings;
        private FilteringSettings _filteringSettings;
        private RenderStateBlock _renderStateBlock;
        public HighlightPass(Settings settings)
        {
            UpdateSettings(settings);
        }

        public void UpdateSettings(Settings settings)
        {
            _settings = settings;
            renderPassEvent = _settings.passEvent;

            RenderQueueRange range = RenderQueueRange.opaque;
            switch (_settings.renderQueue)
            {
                case Settings.RenderQueueMode.Transparent:
                    range = RenderQueueRange.transparent;
                    break;
                case Settings.RenderQueueMode.All:
                    range = RenderQueueRange.all;
                    break;
            }

            _filteringSettings = new FilteringSettings(range, _settings.layerMask, _settings.renderingLayerMask);

            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            if (_settings.overrideDepthState)
            {
                _renderStateBlock.mask |= RenderStateMask.Depth;
                _renderStateBlock.depthState = new DepthState(_settings.depthWrite, _settings.depthCompare);
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_settings.overrideMaterial == null)
                return;

            SortingCriteria sorting = SortingCriteria.CommonOpaque;
            if (_settings.renderQueue == Settings.RenderQueueMode.Transparent)
                sorting = SortingCriteria.CommonTransparent;

            DrawingSettings drawingSettings = CreateDrawingSettings(ShaderTags, ref renderingData, sorting);
            drawingSettings.overrideMaterial = _settings.overrideMaterial;
            drawingSettings.overrideMaterialPassIndex = _settings.overrideMaterialPassIndex;

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings, ref _renderStateBlock);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_settings.overrideMaterial == null)
                return;

            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("InteractableHighlight", out var passData, profilingSampler))
            {
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Write);

                TextureHandle mainShadowsTexture = resourceData.mainShadowsTexture;
                if (mainShadowsTexture.IsValid())
                    builder.UseTexture(mainShadowsTexture, AccessFlags.Read);

                TextureHandle additionalShadowsTexture = resourceData.additionalShadowsTexture;
                if (additionalShadowsTexture.IsValid())
                    builder.UseTexture(additionalShadowsTexture, AccessFlags.Read);

                TextureHandle[] dBufferHandles = resourceData.dBuffer;
                for (int i = 0; i < dBufferHandles.Length; i++)
                {
                    TextureHandle dBuffer = dBufferHandles[i];
                    if (dBuffer.IsValid())
                        builder.UseTexture(dBuffer, AccessFlags.Read);
                }

                TextureHandle ssaoTexture = resourceData.ssaoTexture;
                if (ssaoTexture.IsValid())
                    builder.UseTexture(ssaoTexture, AccessFlags.Read);

                SortingCriteria sorting = SortingCriteria.CommonOpaque;
                if (_settings.renderQueue == Settings.RenderQueueMode.Transparent)
                    sorting = SortingCriteria.CommonTransparent;

                DrawingSettings drawingSettings = RenderingUtils.CreateDrawingSettings(ShaderTags, renderingData, cameraData, lightData, sorting);
                drawingSettings.overrideMaterial = _settings.overrideMaterial;
                drawingSettings.overrideMaterialPassIndex = _settings.overrideMaterialPassIndex;

                var listParams = new RendererListParams(renderingData.cullResults, drawingSettings, _filteringSettings);
                passData.rendererListHdl = renderGraph.CreateRendererList(listParams);

                builder.UseRendererList(passData.rendererListHdl);
                builder.AllowGlobalStateModification(true);

                builder.SetRenderFunc((PassData data, RasterGraphContext rgContext) =>
                {
                    rgContext.cmd.DrawRendererList(data.rendererListHdl);
                });
            }
        }

        private class PassData
        {
            internal RendererListHandle rendererListHdl;
        }
    }
}
