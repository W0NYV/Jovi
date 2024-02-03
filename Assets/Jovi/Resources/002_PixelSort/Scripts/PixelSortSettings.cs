using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace W0NYV.Jovi
{
    [System.Serializable]
    public class PixelSortSettings
    {
        [Range(0, 1f)] public float Threshold;
    }
}
