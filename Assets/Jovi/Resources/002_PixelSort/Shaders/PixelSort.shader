Shader "Hidden/Jovi/PixelSort"
{
    Properties
    {
        _Threshold("Threshold", Range(0.0, 1.0)) = 1.0 
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off

        // Using Full Screen Pass Renderer Feature.
        // Re:Reimplemented pixelsort shader based on https://www.shadertoy.com/view/NdGXWD

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment PixelSort0

            #include "./PixelSort.hlsl"

            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment PixelSort1

            #include "./PixelSort.hlsl"

            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment PixelSort2

            #include "./PixelSort.hlsl"

            ENDHLSL
        }
    }
}