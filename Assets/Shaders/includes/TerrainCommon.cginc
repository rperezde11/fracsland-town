#ifndef BUMP_TERRAIN_INCLUDE
#define BUMP_TERRAIN_INCLUDE

#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "../includes/Common.cginc"
#include "../includes/BumpCommon.cginc"

#pragma multi_compile _ _TERRAIN_NORMAL_MAP

#define SHADOWS_FWD_BASE (defined(SHADOWS_SCREEN))
#define SHADOWS_FWD_ADD  (defined(SHADOWS_DEPTH) && defined(SPOT) || defined(SHADOWS_CUBE) && defined(POINT))

uniform sampler2D _Control;	
uniform float4 _Control_ST;

uniform sampler _Splat0, _Splat1, _Splat2, _Splat3;
uniform float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
uniform float4 _Normal0_ST;

#if _TERRAIN_NORMAL_MAP
uniform float _Bumpiness;
uniform float _Tiling;
uniform sampler _Normal0, _Normal1, _Normal2, _Normal3;
#endif

#if _TERRAIN_PARALLAX_MAP
uniform float _ParallaxFactor;
uniform sampler2D _Displacement0, _Displacement1, _Displacement2, _Displacement3;
#endif

struct SplatMapUV
{
	float2 uv_Splat0  : TEXCOORD0;
	float2 uv_Splat1  : TEXCOORD1;
	float2 uv_Splat2  : TEXCOORD2;
	float2 uv_Splat3  : TEXCOORD3;
	float2 tc_Control : TEXCOORD4;
};

struct SplatMapTex
{
	sampler2D tex0;
	sampler2D tex1;
	sampler2D tex2;
	sampler2D tex3;
};

#define SET_SPLAT_TEXTURES(name,tex) SplatMapTex name; name##.tex0 = tex##0; name##.tex1 = tex##1; name##.tex2 = tex##2; name##.tex3 = tex##3;

struct BumpVectors
{
	float3 _TangentWorld;
	float3 _BinormalWorld;
	float3 normalWorld;
};

struct appdata_terrain
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float2 texcoords : TEXCOORD0;
};

struct v2f_terrain
{
	float4 pos : SV_POSITION;

	float2 pack_c : TEXCOORD0;
	float4 pack_0 : TEXCOORD1;
	float4 pack_1 : TEXCOORD2;

	float4 posWorld    : TEXCOORD3;
	float3 normalWorld : TEXCOORD4;

	#if _TERRAIN_NORMAL_MAP
	float3 tangentWorld  : TEXCOORD5;
	float3 binormalWorld : TEXCOORD6;
	#endif

	#if _TERRAIN_PARALLAX_MAP
	float3 viewSurface : TEXCOORD7;	
	#endif

	#if SHADOWS_FWD_BASE || SHADOWS_FWD_ADD
	SHADOW_COORDS(8)
	#endif
};

v2f_terrain vertTerrain(appdata_terrain v)
{
	v2f_terrain o;

	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

	o.posWorld    = mul(unity_ObjectToWorld, v.vertex);
	o.normalWorld = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject)).xyz;

	SplatMapUV IN;
	IN.tc_Control =	TRANSFORM_TEX(v.texcoords, _Control);

	o.pack_c.xy = IN.tc_Control;
	o.pack_0.xy = TRANSFORM_TEX(v.texcoords, _Splat0);
	o.pack_0.zw = TRANSFORM_TEX(v.texcoords, _Splat1);
	o.pack_1.xy = TRANSFORM_TEX(v.texcoords, _Splat2);
	o.pack_1.zw = TRANSFORM_TEX(v.texcoords, _Splat3);

	#if defined(_TERRAIN_NORMAL_MAP) || defined(_TERRAIN_PARALLAX_MAP)
	o.tangentWorld = normalize(mul(unity_ObjectToWorld, cross(v.normal, half3(0,0,1))));
	o.binormalWorld = normalize(cross(o.normalWorld, o.tangentWorld)) * -1;

	float3 tangentObject = mul(unity_WorldToObject, half4(o.tangentWorld, 0)).xyz;
	float3 binormalObject = cross(v.normal, tangentObject).xyz * -1;
	#endif

	#if _TERRAIN_PARALLAX_MAP
	float3x3 localToSurfaceTranspose = float3x3(
		tangentObject,
		binormalObject,
		v.normal
	);

	float3 objViewDir = mul(unity_WorldToObject, _WorldSpaceCameraPos - o.posWorld).xyz;
	o.viewSurface = mul(localToSurfaceTranspose, objViewDir);
	#endif

	#if SHADOWS_FWD_BASE || SHADOWS_FWD_ADD
	TRANSFER_SHADOW(o);
	#endif

	o.posWorld.w = GetFogFactor(o.pos);

	return o;
}


void SplatMapVal(float4 splatControl, SplatMapTex TEX, SplatMapUV UV, float scale, float offset, out float4 val)
{
	val = 0.0;
	val = splatControl.r * tex2D(TEX.tex0,  scale * UV.uv_Splat0 + offset);
	val += splatControl.g * tex2D(TEX.tex1, scale * UV.uv_Splat1 + offset);
	val += splatControl.b * tex2D(TEX.tex2, scale * UV.uv_Splat2 + offset);
	val += splatControl.a * tex2D(TEX.tex3, scale * UV.uv_Splat3 + offset);
}

void SplatMapDisplacementVal(float4 splatControl, SplatMapTex TEX, SplatMapUV UV, float scale, float offset, out float4 val)
{
	val = 0.0;
	val += splatControl.r * tex2D(TEX.tex0, scale * (UV.uv_Splat0%0.9999) + offset);
	val += splatControl.g * tex2D(TEX.tex1, scale * (UV.uv_Splat1%0.9999) + offset);
	val += splatControl.b * tex2D(TEX.tex2, scale * (UV.uv_Splat2%0.9999) + offset);
	val += splatControl.a * tex2D(TEX.tex3, scale * (UV.uv_Splat3%0.9999) + offset);
}
#endif