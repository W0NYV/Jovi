using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace W0NYV.Jovi.Boids
{
    [System.Serializable]
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    internal struct BoidData
    {
        internal Vector3 Velocity;
        internal Vector3 Position;
    }
}