#ifndef UTILS_LIGHT_INCLUDE
#define UTILS_LIGHT_INCLUDE

#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "UnityStandardBRDF.cginc"

float GetLightRange(float3 worldPos)
{
	#ifdef POINT
	float3 lightCoords = mul(unity_WorldToLight, float4(worldPos, 1.0)).xyz;
	float3 lightDir = _WorldSpaceLightPos0.xyz - worldPos.xyz;
	return length(lightDir) / length(lightCoords);
	#endif

	return 1.0;
}

float GetLightAttenuation(float4 worldPos, float factor)
{
	#ifndef DIRECTIONAL
	float3 lightCoords = mul(unity_WorldToLight, worldPos).xyz;
	return saturate(factor * (1 - dot(lightCoords, lightCoords)));
	#endif

	return 0;
}

float GetAtten(float3 worldPos, float factor)
{
	#ifdef POINT
	unityShadowCoord3 lightCoord = mul(unity_WorldToLight, unityShadowCoord4(worldPos, 1)).xyz * factor;
	lightCoord.y *= 0.1;
	return (tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL);
	#endif

	return 1.0;
}

// Sample Unity's LUT texture for specular intensity.
float GetSpecularIntensityFromLUT(float3 normal, float3 halfVec, half roughness)
{
	// 16 is the value use on Unity shader to sample unity_NHxRoughness
	// I guess some experimentation has been done.
	return 16 * tex2D(unity_NHxRoughness, float2(Pow4(saturate(dot(normal, halfVec))), roughness)).r;
}

half3 GetSpecularColorByMetallic(half3 albedo, float oneMinusReflectivity, float3 normal, float3 halfVec, half roughness)
{
	float specIntensity = GetSpecularIntensityFromLUT(normal, halfVec, roughness);

	half3 specularColor = lerp(albedo, _LightColor0.rgb, oneMinusReflectivity);

	return specIntensity * specularColor;
}

#endif