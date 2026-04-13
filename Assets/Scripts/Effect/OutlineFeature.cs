using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    class OutlineRenderPass : ScriptableRenderPass
    {
        private readonly Material m_OutlineMaterial;
        private RTHandle m_OutlineMaskRT;
        private readonly FilteringSettings filteringSettings;
        private readonly MaterialPropertyBlock materialPropertyBlock;
        private static readonly int s_ShaderProp_OutlineMask = Shader.PropertyToID("_OutlineMask");

        private static readonly List<ShaderTagId> shaderTagIds = new List<ShaderTagId>()
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly")
        };

        public OutlineRenderPass(Material outlineMaterial)
        {
            m_OutlineMaterial = outlineMaterial;
            // Configures where the render pass should be injected.
            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
            filteringSettings = new FilteringSettings(RenderQueueRange.all, renderingLayerMask: 2);
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        public void Dispose()
        {
            m_OutlineMaskRT?.Release();
            m_OutlineMaskRT = null;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ResetTarget();
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.msaaSamples = 1;
            desc.depthBufferBits = 0;
            desc.colorFormat = RenderTextureFormat.ARGB32;
            RenderingUtils.ReAllocateIfNeeded(ref m_OutlineMaskRT, desc, name: "_OutlineMaskRT");
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("Outline Command");
            cmd.SetRenderTarget(m_OutlineMaskRT);
            cmd.ClearRenderTarget(true, true, Color.clear);

            // Draw all the objects needing the outline effect.
            var drawingSettings = CreateDrawingSettings(shaderTagIds, ref renderingData, SortingCriteria.None);
            var rendererListParams = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
            var list = context.CreateRendererList(ref rendererListParams);
            cmd.DrawRendererList(list);

            // Draw outline
            cmd.SetRenderTarget(renderingData.cameraData.renderer.cameraColorTargetHandle);
            materialPropertyBlock.SetTexture(s_ShaderProp_OutlineMask, m_OutlineMaskRT);
            cmd.DrawProcedural(Matrix4x4.identity, m_OutlineMaterial, 0, MeshTopology.Triangles, 3, 1, materialPropertyBlock);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    [SerializeField] private Material m_OutlineMaterial;
    OutlineRenderPass m_OutlineRenderPass;

    private bool IsMaterialValid => m_OutlineMaterial && m_OutlineMaterial.shader && m_OutlineMaterial.shader.isSupported;

    /// <inheritdoc/>
    public override void Create()
    {
        //Debug.Log($"{m_OutlineMaterial != null}, {m_OutlineMaterial.shader != null}, {m_OutlineMaterial.shader.isSupported}");

        if (!IsMaterialValid)
        {
            return;
        }

        m_OutlineRenderPass = new OutlineRenderPass(m_OutlineMaterial);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_OutlineRenderPass == null)
        {
            return;
        }

        renderer.EnqueuePass(m_OutlineRenderPass);
    }

    protected override void Dispose(bool disposing)
    {
        m_OutlineRenderPass?.Dispose();
    }
}


