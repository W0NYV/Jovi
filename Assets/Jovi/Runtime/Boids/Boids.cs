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

        [SerializeField] private float _separateNeighborhoodRadius = 1.0f;
        [SerializeField] private float _separateWeight = 3.0f;

        [SerializeField] private float _alignmentNeighborhoodRadius = 2.0f;
        [SerializeField] private float _alignmentWeight = 1.0f;

        [SerializeField] private float _cohesionNeighborhoodRadius = 2.0f;
        [SerializeField] private float _cohesionWeight = 1.0f;

        [SerializeField] private float _avoidWallWeight = 10.0f;

        [SerializeField] private float _maxSpeed = 5.0f;
        [SerializeField] private float _maxSteelForce = 0.5f;

        [SerializeField] private Vector3 _wallCenter = Vector3.zero;
        [SerializeField] private Vector3 _wallSize = new Vector3(32.0f, 32.0f, 32.0f);

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
                boidDataArray[i].Position = Random.insideUnitSphere * 0.25f;
                boidDataArray[i].Velocity = Random.insideUnitSphere * 1.0f;

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

            cs.SetInt("_BoidsCount", _boidCount);

            cs.SetFloat("_SeparateNeighborhoodRadius", _separateNeighborhoodRadius);
            cs.SetFloat("_SeparateWeight", _separateWeight);

            cs.SetFloat("_AlignmentNeighborhoodRadius", _alignmentNeighborhoodRadius);
            cs.SetFloat("_AlignmentWeight", _alignmentWeight);

            cs.SetFloat("_CohesionNeighborhoodRadius", _cohesionNeighborhoodRadius);
            cs.SetFloat("_CohesionWeight", _cohesionWeight);

            cs.SetFloat("_AvoidWallWeight", _avoidWallWeight);
            
            cs.SetFloat("_MaxSpeed", _maxSpeed);
            cs.SetFloat("_MaxSteelForce", _maxSteelForce);

            cs.SetVector("_WallCenter", _wallCenter);
            cs.SetVector("_WallSize", _wallSize);

            cs.SetBuffer(id, "_BoidDataBufferRead", _boidDataBuffer);
            cs.SetBuffer(id, "_BoidForceBufferWrite", _boidForceBuffer);
            cs.Dispatch(id, threadGroupSize, 1, 1);

            id = cs.FindKernel("Integrate");

            cs.SetFloat("_DeltaTime", Time.deltaTime);

            cs.SetBuffer(id, "_BoidForceBufferRead", _boidForceBuffer);
            cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
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