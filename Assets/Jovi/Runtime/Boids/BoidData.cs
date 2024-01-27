using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace W0NYV.Jovi.Boids
{
    [System.Serializable]
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    public struct BoidData
    {
        public Vector3 Velocity;
        public Vector3 Position;
    }
}