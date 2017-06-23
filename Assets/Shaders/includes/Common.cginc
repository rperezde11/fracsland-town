#ifndef COMMON_INCLUDE
#define COMMON_INCLUDE

#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "../includes/BumpCommon.cginc"
#include "../includes/FogCommon.cginc"

#define SHADOWS_FWD_BASE (defined(SHADOWS_SCREEN))
#define SHADOWS_FWD_ADD  (defined(SHADOWS_DEPTH) && defined(SPOT) || defined(SHADOWS_CUBE) && defined(POINT))
#define USE_EXTRA_MAP (_EMISSION_ON || _SPECULARMAP_ON)
#define TANGENT_MAP (_BUMPMAP || _PARALLAX_MAP)

struct appdata_basic
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	fixed2 coord  : TEXCOORD0;
	
	#if TANGENT_MAP
	float4 tangent : TANGENT;
	#endif
};

struct v2f_basic
{
	float4 pos         : SV_POSITION;
	fixed2 texcoords   : TEXCOORD0;
	float4 posWorld    : TEXCOORD1;
	float3 normalWorld : TEXCOORD2;

	#if _BUMPMAP
	BUMP_COORDS(3, 4)
	#endif

	#ifdef _PARALLAX_MAP
	float3 viewSurface : TEXCOORD5;
	#endif

	#if SHADOWS_FWD_BASE || SHADOWS_FWD_ADD
	SHADOW_COORDS(6)
	#endif

	#if !_REFLECTION_NONE
	float3 reflection : TEXCOORD7;
	#endif
};

#include "../includes/UtilsReflectivity.cginc"

v2f_basic vertBasic(appdata_basic v)
{
	v2f_basic o;

	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.texcoords = v.coord;
	o.posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.normalWorld = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject).xyz);

	#if _BUMPMAP
	TRANSFER_BUMP(o);
	#endif

	#if _PARALLAX_MAP
	float3 binormalObject = cross(v.normal, v.tangent.xyz) * v.tangent.w;

	float3x3 localToSurface = float3x3(
		v.tangent.xyz,
		binormalObject,
		v.normal
	);

	float3 objViewDir = mul(unity_WorldToObject, _WorldSpaceCameraPos - o.posWorld).xyz;
	o.viewSurface = mul(localToSurface, objViewDir);
	#endif

	#if SHADOWS_FWD_BASE || SHADOWS_FWD_ADD
	TRANSFER_SHADOW(o);
	#endif

	#if !_REFLECTION_NONE && REFLECTION_OPTIMIZE
	float3 view = normalize(_WorldSpaceCameraPos.xyz - o.posWorld);
	o.reflection = ComputeReflexDirection(view, o.normalWorld, o.posWorld);
	#endif

	o.posWorld.w = GetFogFactor(o.pos);

	return o;
}

void InitFragmentVectors(float4 posWorld, inout float3 normalWorld, out float3 l, out float atten)
{
	atten = 1.0;

	l = _WorldSpaceLightPos0.xyz;

	if (_WorldSpaceLightPos0.w != 0.0) // not directional lights
	{
		l -= posWorld.xyz;
		
		#ifdef POINT
			float3 lightCoord = mul(unity_WorldToLight, float4(posWorld.xyz, 1)).xyz;
			atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
		#endif

		#ifdef SPOT
			float4 lightCoord = mul(unity_WorldToLight, float4(posWorld.xyz, 1));
			atten = (lightCoord.z > 0) * UnitySpotCookie(lightCoord) * UnitySpotAttenuate(lightCoord.xyz);
		#endif
	}

	l = normalize(l);
	normalWorld = normalize(normalWorld);
}
	
void InitFragmentVectors(float4 posWorld, inout float3 normalWorld, out float3 l, out float3 v, out float atten)
{
	InitFragmentVectors(posWorld, normalWorld, l, atten);

	v = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
}

void InitFragmentVectors(float4 posWorld, inout float3 normalWorld, out float3 l, out float3 v, out float3 h, out float atten)
{
	InitFragmentVectors(posWorld, normalWorld, l, v, atten);

	h = normalize(l + v);
}

#endif