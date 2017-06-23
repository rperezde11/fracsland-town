#ifndef UTILS_BUMP_INCLUDE
#define UTILS_BUMP_INCLUDE

#include "../includes/BumpCommon.cginc"

float3 UnpackNormals(sampler2D texN, float2 uv, float bumpiness)
{
	float3 normal;
	
	normal.xy = 2 * tex2D(texN, uv).ag - 1;
	normal.xy *= bumpiness;
	normal.z = 1 - 0.5 * dot(normal.xy, normal.xy);

	return normal;
}

void CalculateBareParallaxTexcoords(ParallaxInfo info, sampler2D heightMap, inout float2 texcoords, float4 texcoords_ST)
{
	#if SIMPLE_PARALLAX
	float height = tex2D(heightMap, texcoords.xy * texcoords_ST.xy + texcoords_ST.zw).x;
	float2 displacement = ParallaxOffset(height, info.heightFactor, info.view);
	texcoords += displacement;
	#endif
}

void CalculateSteepParallaxTexcoords(ParallaxInfo info, sampler2D heightMap, inout float2 texcoords, float4 texcoords_ST)
{
	#if COMPLEX_PARALLAX
	float weight = 1 - (info.view.z / length(info.view));
	int layers = info.minLayers + (info.maxLayers - info.minLayers) * weight;
	texcoords.xy = GetParallaxSteepUV(layers, info.view, texcoords, info.heightFactor, info.maxLayers, heightMap, texcoords_ST);
	#endif
}

void CalculateParallaxTexcoords(float3 view, float heightFactor, float maxDistance, inout float2 texcoords, sampler2D heightMap, float4 texcoords_ST)
{
	bool farEnough;
	ParallaxInfo info;

	#if NO_PARALLAX
	return;
	#endif

	
	if (length(view) > maxDistance) return;
	
	info.view = view;
	info.heightFactor = heightFactor;

	#if SIMPLE_PARALLAX
	CalculateBareParallaxTexcoords(info, heightMap, texcoords, texcoords_ST);
	#elif COMPLEX_PARALLAX
		
		#if UNITY_PASS_FORWARDADD
		info.minLayers = 8; info.maxLayers = 12;
		#else
		info.minLayers = 12; info.maxLayers = 18;
		#endif
		
		CalculateSteepParallaxTexcoords(info, heightMap, texcoords, texcoords_ST);
	#endif
}


float3 CalculateBumpNormals(half bumpiness, v2f_basic v, sampler2D normals, float4 normals_ST)
{
	float3 computedNormals;
	
	#if _BUMPMAP
	float4 texN = tex2D(normals, v.texcoords.xy * normals_ST.xy + normals_ST.zw);
	computedNormals = BUMP_NORMAL(v, texN, bumpiness);
	#else
	computedNormals = v.normalWorld;
	#endif

	return computedNormals;
}


#endif