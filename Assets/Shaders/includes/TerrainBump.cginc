#ifndef UTILS_TERRAIN_BUMP_INCLUDE
#define UTILS_TERRAIN_BUMP_INCLUDE

#include "../includes/TerrainCommon.cginc"
#include "../includes/BumpCommon.cginc"

void CalculateBareParallaxTexcoords(ParallaxInfo info, float4 splatControl, inout SplatMapUV IN, float tiling, float offset)
{
	half4 height;
	float displacement;

	SET_SPLAT_TEXTURES(TEX_P, _Displacement);
	SplatMapDisplacementVal(splatControl, TEX_P, IN, tiling, offset, height);

	displacement = ParallaxOffset(height.r, info.heightFactor, info.view);

	IN.uv_Splat0 += displacement;
	IN.uv_Splat1 += displacement;
	IN.uv_Splat2 += displacement;
	IN.uv_Splat3 += displacement;
}

void CalculateSteepParallaxTexcoords(ParallaxInfo info, float4 splatControl, inout SplatMapUV IN, float tiling, float offset)
{
	#if COMPLEX_PARALLAX

	float weight = 1 - (info.view.z / length(info.view));
	int layers = info.minLayers + (info.maxLayers - info.minLayers) * weight;

	float4 height;

	SET_SPLAT_TEXTURES(TEX_P, _Displacement);

	IN.uv_Splat0 = GetParallaxSteepUV(layers, info.view, IN.uv_Splat0%0.9999, info.heightFactor, info.maxLayers, TEX_P.tex0, float4(tiling, tiling, offset, offset));
	IN.uv_Splat1 = GetParallaxSteepUV(layers, info.view, IN.uv_Splat1%0.9999, info.heightFactor, info.maxLayers, TEX_P.tex1, float4(tiling, tiling, offset, offset));
	IN.uv_Splat2 = GetParallaxSteepUV(layers, info.view, IN.uv_Splat2%0.9999, info.heightFactor, info.maxLayers, TEX_P.tex2, float4(tiling, tiling, offset, offset));
	IN.uv_Splat3 = GetParallaxSteepUV(layers, info.view, IN.uv_Splat3%0.9999, info.heightFactor, info.maxLayers, TEX_P.tex3, float4(tiling, tiling, offset, offset));

	#endif
}

void CalculateParallaxTexcoords(float3 view, float heightFactor, float4 splatControl, inout SplatMapUV IN, float tiling, float offset)
{
	ParallaxInfo info;

	info.view = view;
	info.heightFactor = heightFactor;

	#if SIMPLE_PARALLAX
	CalculateBareParallaxTexcoords(info, splatControl, IN, tiling, offset);
	#elif COMPLEX_PARALLAX
	info.minLayers = 8;
	info.maxLayers = 15;
	CalculateSteepParallaxTexcoords(info, splatControl, IN, tiling, offset);
	#endif
}


float3 CalculateBumpNormals(float4 splatControl, half bumpiness, v2f_terrain v, SplatMapUV IN, float tiling, float offset)
{
	float4 texN;

	#if _TERRAIN_NORMAL_MAP
	
	SET_SPLAT_TEXTURES(TEX_N, _Normal);
	SplatMapVal(splatControl, TEX_N, IN, tiling, offset, texN);

	// This is only to pass a struct containing 
	// the fields to function BUMP_NORMAL
	BumpVectors b;
	b._TangentWorld  = v.tangentWorld;
	b._BinormalWorld = v.binormalWorld;
	b.normalWorld    = v.normalWorld;

	return BUMP_NORMAL(b, texN, bumpiness);
	#endif

	return v.normalWorld;
}

#endif