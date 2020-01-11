using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

public class OutlineFeature : ScriptableRendererFeature
{
    class OutlinePass : ScriptableRenderPass
    {
        private RenderTargetIdentifier m_source { get; set; }
        private RenderTargetHandle m_destination { get; set; }
        public Material m_outlineMaterial = null;
        RenderTargetHandle m_temporaryColorTexture;

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            this.m_source = source;
            this.m_destination = destination;
        }

        //Constructor
        public OutlinePass(Material outlineMaterial)
        {
            this.m_outlineMaterial = outlineMaterial;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            //base.Configure(cmd, cameraTextureDescriptor);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("_OutlinePass");
            RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDescriptor.depthBufferBits = 0;

            if (m_destination == RenderTargetHandle.CameraTarget)
            {
                cmd.GetTemporaryRT(m_temporaryColorTexture.id, opaqueDescriptor, FilterMode.Point);
                Blit(cmd, m_source, m_temporaryColorTexture.Identifier(), m_outlineMaterial, 0);
                Blit(cmd, m_temporaryColorTexture.Identifier(), m_source);
            }
            else
                Blit(cmd, m_source, m_destination.Identifier(), m_outlineMaterial, 0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

          //  throw new System.NotImplementedException();
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (m_destination == RenderTargetHandle.CameraTarget)
                cmd.ReleaseTemporaryRT(m_temporaryColorTexture.id);
            //base.FrameCleanup(cmd);
        }
    }

    [System.Serializable]
    public class OutlineSettings
    {
        public Material outlineMaterial = null;
    }

    public OutlineSettings settings = new OutlineSettings();
    OutlinePass outlinePass;
    RenderTargetHandle m_outlineTexture;

    public override void Create()
    {
        outlinePass = new OutlinePass(settings.outlineMaterial);
        outlinePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        m_outlineTexture.Init("_OutlineTexture");
       // throw new System.NotImplementedException();)
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(settings.outlineMaterial == null)
        {
            Debug.LogWarningFormat("Missing Outline Material");
            return;
        }

        outlinePass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
        renderer.EnqueuePass(outlinePass);
        //throw new System.NotImplementedException();
    }
}
