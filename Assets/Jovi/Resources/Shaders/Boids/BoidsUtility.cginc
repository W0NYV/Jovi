#ifndef BOIDSUTILITY_INCLUDED
#define BOIDSUTILITY_INCLUDED

struct BoidData
{
    float3 velocity;
    float3 position;
};

float3 limit(float3 vec, float max)
{
    float length = sqrt(dot(vec, vec));
    return (length > max && length > 0) ? vec.xyz * (max / length) : vec.xyz;
}

float3 avoidWall(float3 position, float3 wallCenter, float3 wallSize)
{
    float3 wc = wallCenter.xyz;
    float3 ws = wallSize.xyz;
    float3 acc = float3(0, 0, 0);

    acc.x = (position.x < wc.x - ws.x * 0.5) ? acc.x + 1.0 : acc.x;
    acc.x = (position.x > wc.x + ws.x * 0.5) ? acc.x - 1.0 : acc.x;

    acc.y = (position.y < wc.y - ws.y * 0.5) ? acc.y + 1.0 : acc.y;
    acc.y = (position.y > wc.y + ws.y * 0.5) ? acc.y - 1.0 : acc.y;

    acc.z = (position.z < wc.z - ws.z * 0.5) ? acc.z + 1.0 : acc.z;
    acc.z = (position.z > wc.z + ws.z * 0.5) ? acc.z - 1.0 : acc.z;

    return acc;
}

#endif
