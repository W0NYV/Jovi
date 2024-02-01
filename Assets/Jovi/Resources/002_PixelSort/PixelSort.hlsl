#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Re:Reimplemented : https://www.shadertoy.com/view/NdGXWD

Texture2D _BlitTexture;

float grey(float3 c)
{
    return dot(c, float3(0.299, 0.587, 0.114));
}

void draw_float(float4 src, float threshold, float2 uv, SamplerState Sampler, out float4 dest)
{
    // float g = grey(src.rgb);

    // dest = float4(src.r, src.g, src.b, src.a);

    UnitySamplerState hoge;

    dest = SAMPLE_TEXTURE2D_X(_BlitTexture, uv);
}