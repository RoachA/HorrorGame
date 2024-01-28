using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VHSFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class VHSSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        [Header("Chroma Delay")]
        public float _DelayAmount = 0.003f;
        public float _DelayOffset = 0.01f;
        public float _ChromaOffset = 0.002f;
        public Vector2 _ChromaVector = new Vector2(20, 15);
        public float _Sharpness = 1f;

        [Header("Dot Crawl")]
        public Vector2 _DotCrawlVector = new Vector2(50, 35);
        public float _DotCrawlSpeed = 1.0f;
        public float _DotCrawlAmount = 0.05f;

        [Header("Ringing")]
        public float _RingingAmount = 1f;

        [Header("Chrominance Noise")]
        public Texture2D _noiseTexture;
        public Vector2 _noiseDistribution = new Vector2(0, 1);
        public float _NoiseAmount = 0.02f;
        public float _NoiseSpeed = 1.0f;

        [Header("Spatial Mosaic Artifacts")]
        public float _MosaicSize = 10;
        public float _Compression = 0.1f;
        public Vector2 _MosaicDistribution = new Vector2(0, 1);
        public Vector2 _SobelStep = new Vector2(0.001f, 0.001f);
    }

    [SerializeField] private VHSSettings Settings;
    private VHSPass VHSpass;

    public override void Create()
    {
        VHSpass = new VHSPass(Settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(VHSpass);
    }
}

class VHSPass : ScriptableRenderPass
{
    public Material vhsMat;
    public VHSFeature.VHSSettings settings;
    private RenderTargetIdentifier colorBuffer, pixelBuffer;
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

    public VHSPass(VHSFeature.VHSSettings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
        if (vhsMat == null) vhsMat = CoreUtils.CreateEngineMaterial("PostProcess/VHS");
    }
    
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        
        vhsMat.SetFloat("_DelayAmount", settings._DelayAmount);
        vhsMat.SetFloat("_DelayOffset", settings._DelayOffset);
        vhsMat.SetFloat("_ChromaOffset", settings._ChromaOffset);
        vhsMat.SetFloat("_Sharpness", settings._Sharpness);
        vhsMat.SetVector("_ChromaVector", settings._ChromaVector);
        
        vhsMat.SetVector("_DotCrawlVector", settings._DotCrawlVector);
        vhsMat.SetFloat("_DotCrawlSpeed", settings._DotCrawlSpeed);
        vhsMat.SetFloat("_DotCrawlAmount", settings._DotCrawlAmount);
        
        vhsMat.SetFloat("_RingingAmount", settings._RingingAmount);

        vhsMat.SetTexture("_NoiseTex", settings._noiseTexture);
        vhsMat.SetVector("_NoiseDistribution", settings._noiseDistribution);
        vhsMat.SetFloat("_NoiseAmount", settings._NoiseAmount);
        vhsMat.SetFloat("_NoiseSpeed", settings._NoiseSpeed);
        
        vhsMat.SetVector("_SobelStep", settings._SobelStep);
        vhsMat.SetFloat("_MosaicSize", settings._MosaicSize);
        vhsMat.SetFloat("_Compression", settings._Compression);
        vhsMat.SetVector("_MosaicDistribution", settings._MosaicDistribution);

        descriptor.height = Screen.height;
        descriptor.width = Screen.width;
        
        cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Bilinear);
        pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
        
       // base.OnCameraSetup(cmd, ref renderingData);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        /*if (vhsMat == null)
            return;*/
            
        CommandBuffer cmd = CommandBufferPool.Get();

        using (new ProfilingScope(cmd, new ProfilingSampler("VHS Pass")))
        {
            Blit(cmd, colorBuffer, pixelBuffer, vhsMat);
            Blit(cmd, pixelBuffer, colorBuffer);
        }

        // cmd.Blit(RenderTargetHandle.CameraTarget.Identifier(), BuiltinRenderTextureType.CameraTarget, vhsMat);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentException("cmd");
        cmd.ReleaseTemporaryRT(pixelBufferID);
    }
}
