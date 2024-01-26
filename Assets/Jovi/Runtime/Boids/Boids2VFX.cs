using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace W0NYV.Jovi.Boids
{
    [RequireComponent(typeof(Boids))]
    internal class Boids2VFX : MonoBehaviour
    {
        #region Private fields

        [SerializeField] private VisualEffect _visualEffect;

        #endregion
        
        #region Internal methods

        internal void SetGraphicsBuffer(GraphicsBuffer graphicsBuffer)
        {
            _visualEffect.SetGraphicsBuffer("GraphicsBuffer", graphicsBuffer);
        }

        #endregion
    }
}
