#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

float _Threshold;
TEXTURE2D(_SrcTex);
SAMPLER(sampler_SrcTex);

float4 _TexelSize;

half gray(float c)
{
    return dot(c, float3(0.299, 0.587, 0.114));
}

float4 draw(float2 texcoord)
{
    half4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, texcoord);

    half g = gray(col.rgb);
    if (g < _Threshold)
    {
        return float4(texcoord, 1.0, 1.0);
    }

    float angle = 0.0;

    float rank = 0.0;
    float2 headPos;

    //これを解像度で割るとどうなる？
    float2 dir = float2(cos(angle), sin(angle)) / _TexelSize.xy;

    //ピクセルソートの開始位置を探るためのやつ

    //左
    [loop]
    for (float i = 0.0; i < 9999.0; i += 1.0)
    {
        //これのおかげでdir方向にぐんぐん進む
        float2 uv2 = texcoord - dir * i;

        //端だったら
        if (uv2.x < 0.0 || uv2.y < 0.0 || uv2.x >= 1.0 || uv2.y >= 1.0)
        {
            headPos = uv2;
            break;
        }

        //グレースケールでしきい値より小さければ
        half3 col_i = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv2).rgb;
        half g_i = gray(col_i);
        if (g_i < _Threshold)
        {
            headPos = uv2;
            break;
        }

        if (g_i <= g)
        {
            rank += 1.0;
        }

    }

    //右
    [loop]
    for (float i = 0.0; i < 9999.0; i += 1.0)
    {
        //これのおかげでdir方向にぐんぐん進む
        float2 uv2 = texcoord + dir * i;

        //端だったら
        if (uv2.x < 0.0 || uv2.y < 0.0 || uv2.x >= 1.0 || uv2.y >= 1.0)
        {
            break;
        }

        //グレースケールでしきい値より小さければ
        half3 col_i = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv2).rgb;
        half g_i = gray(col_i);
        if (g_i < _Threshold)
        {
            break;
        }

        if (g_i <= g)
        {
            rank += 1.0;
        }

    }

    float2 dst = headPos + rank * dir;

    return float4(dst, 1.0, 1.0);
}

float4 draw2(float2 texcoord)
{

    float2 uv = texcoord;

    float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv);

    float error = 50.0;
    float angle = 0.0;

    float pixelSize = error / _TexelSize.x;

    float2 dir = float2(cos(angle), sin(angle)) / _TexelSize.xy;

    float2 uv2;
    float2 uv3;

    [loop]
    for (float i = 0.0; i < 999.0; i += 1.0)
    {
        uv2 = uv - dir * i;
        uv3 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv2).xy;
        if (length(uv - uv3) < pixelSize)
        {
            return SAMPLE_TEXTURE2D_X(_SrcTex, sampler_PointClamp, uv2);
        }
    }

    [loop]
    for (float i = 0.0; i < 999.0; i += 1.0)
    {
        uv2 = uv + dir * i;
        uv3 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv2).xy;
        if (length(uv - uv3) < pixelSize)
        {
            return SAMPLE_TEXTURE2D_X(_SrcTex, sampler_PointClamp, uv2);
        }
    }

    //return float4(0.0, 0.0, 0.0, 1.0); 
    return SAMPLE_TEXTURE2D_X(_SrcTex, sampler_PointClamp, col.xy);

}

half4 PixelSort0(Varyings i) : SV_Target
{   
    return draw(i.texcoord);
}

half4 PixelSort1(Varyings i) : SV_Target
{   
    return draw2(i.texcoord);
}

half4 PixelSort2(Varyings i) : SV_Target
{   
    half4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, i.texcoord);
    return col;
}