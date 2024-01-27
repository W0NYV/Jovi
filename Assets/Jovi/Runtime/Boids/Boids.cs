using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.VFX;

namespace W0NYV.Jovi.Boids
{
    public class Boids : MonoBehaviour
    {
        #region Private fields

        private const int SIMULATION_BLOCK_SIZE = 256;

        private GraphicsBuffer _boidForceBuffer;
        private GraphicsBuffer _boidDataBuffer;

        [SerializeField] private ComputeShader _computeShader;
        [SerializeField] private VisualEffect _visualEffect;

        [Header("パラメータ")]
        [SerializeField, Range(256, 32768)] private int _boidCount = 256;
        [SerializeField] private float _maxSpeed;

        #endregion

        #region Private methods

        private void InitGraphicsBuffers()
        {
            _boidDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _boidCount, Marshal.SizeOf(typeof(BoidData)));
            _boidForceBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _boidCount, Marshal.SizeOf(typeof(Vector3)));

            var boidDataArray = new BoidData[_boidCount];
            var forceArray = new Vector3[_boidCount];

            for (int i = 0; i < _boidCount; i++)
            {
                boidDataArray[i].Position = Random.insideUnitSphere * 7f;
                boidDataArray[i].Velocity = Random.insideUnitSphere * 0.1f;

                forceArray[i] = Vector3.zero;
            }

            _boidDataBuffer.SetData(boidDataArray);
            _boidForceBuffer.SetData(forceArray);

            boidDataArray = null;
            forceArray = null;
        }

        private void Simulation()
        {
            ComputeShader cs = _computeShader;

            int threadGroupSize = Mathf.CeilToInt(_boidCount / SIMULATION_BLOCK_SIZE);
            int id = -1;

            id = cs.FindKernel("Force");
            cs.SetBuffer(id, "BoidDataBufferRead", _boidDataBuffer);
            cs.SetBuffer(id, "BoidDataBufferWrite", _boidForceBuffer);
            cs.Dispatch(id, threadGroupSize, 1, 1);

            id = cs.FindKernel("Integrate");
            cs.SetBuffer(id, "BoidDataBufferRead", _boidForceBuffer);
            cs.SetBuffer(id, "BoidDataBufferWrite", _boidDataBuffer);
            cs.Dispatch(id, threadGroupSize, 1, 1);
        }

        private void ReleaseBuffer(GraphicsBuffer graphicsBuffer)
        {
            if(graphicsBuffer != null)
            {
                graphicsBuffer.Release();
                graphicsBuffer = null;
            }
        }

        #endregion

        #region MonoBehaviour implementation

        private void Start() 
        {
            InitGraphicsBuffers();

            _visualEffect.SetGraphicsBuffer("GraphicsBuffer", _boidDataBuffer);
        }

        private void Update() 
        {
            Simulation();
        }

        private void OnDestroy() 
        {
            ReleaseBuffer(_boidDataBuffer);
            ReleaseBuffer(_boidForceBuffer);
        }

        #endregion

    }
}