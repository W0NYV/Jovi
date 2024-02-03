using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace W0NYV.Jovi.PixelSort
{
    public class PixelSortRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private PixelSortSettings _settings;
        [SerializeField] private Shader _shader;
        private Material _material;
        private PixelSortRenderPass _renderPass;

        public override void Create()
        {
            if (_shader == null)
            {
                return;
            }

            _material = new Material(_shader);

            _renderPass = new PixelSortRenderPass(_material, _settings);

            //ここのイベントは変えてもいい
            _renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(_renderPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _renderPass.Dispose();

            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Destroy(_material);
            }
            else
            {
                DestroyImmediate(_material);
            }
            #else
            Destroy(_material)
            #endif
            

        }
    }
}
