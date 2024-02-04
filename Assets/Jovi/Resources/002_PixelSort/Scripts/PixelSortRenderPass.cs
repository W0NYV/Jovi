using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace W0NYV.Jovi.PixelSort
{
    public class PixelSortRenderPass : ScriptableRenderPass
    {
        private PixelSortSettings _settings;
        private Material _material;
        private RenderTextureDescriptor _pixelSortTextureDescriptor;

        private RTHandle _pixelSortTextureHandle;
        private RTHandle _postPixelSortTextureHandle;

        private Vector2 _texelSize;

        // private static readonly int thresholdId = Shader.PropertyID("_Threshold");

        //これコンストラクタだから public じゃなくない？
        public PixelSortRenderPass(Material material, PixelSortSettings settings)
        {
            this._material = material;
            this._settings = settings;

            _pixelSortTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            RTHandle cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

            UpdatePixelSortSettngs(cameraTargetHandle);

            // cmd.SetGlobalTexture("_SrcTex", cameraTargetHandle);


            Blit(cmd, cameraTargetHandle, _pixelSortTextureHandle, _material, 0);

            Blit(cmd, _pixelSortTextureHandle, _postPixelSortTextureHandle, _material, 1);

            Blit(cmd, _postPixelSortTextureHandle, cameraTargetHandle, _material, 2);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            _pixelSortTextureDescriptor.width = cameraTextureDescriptor.width;
            _pixelSortTextureDescriptor.height = cameraTextureDescriptor.height;

            _texelSize = new Vector2(_pixelSortTextureDescriptor.width, _pixelSortTextureDescriptor.height);

            //ディスクリプタが変更されたかどうかをチェックし、必要であればRTHandleを再割り当てする。
            RenderingUtils.ReAllocateIfNeeded(ref _pixelSortTextureHandle, _pixelSortTextureDescriptor);
            RenderingUtils.ReAllocateIfNeeded(ref _postPixelSortTextureHandle, _pixelSortTextureDescriptor);
        }

        public void Dispose()
        {
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Object.Destroy(_material);
            }
            else
            {
                Object.DestroyImmediate(_material);
            }
            #else
            Object.Destroy(_material)
            #endif

            if (_pixelSortTextureHandle != null) _pixelSortTextureHandle.Release();
            if (_postPixelSortTextureHandle != null) _postPixelSortTextureHandle.Release();
        }

        private void UpdatePixelSortSettngs(RTHandle rtHandle)
        {
            if (_material == null) return;

            _material.SetFloat("_Threshold", _settings.Threshold);
            _material.SetVector("_TexelSize", _texelSize);
            _material.SetTexture("_SrcTex", rtHandle);
        }
    }
}
