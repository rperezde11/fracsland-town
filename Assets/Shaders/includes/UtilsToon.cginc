#ifndef UTILS_TOON_INCLUDE
#define UTILS_TOON_INCLUDE

#include "../includes/UtilsLight.cginc"

float GetStairedValue(half x, half step_x)
{
	float step_y = step_x / (1.0 - step_x);
	float val_x = x - fmod(x, step_x);
	
	return  val_x * (1.0 / step_x) * step_y;
}

float GetSquaredStairedValue(half x, half step)
{
	x = GetStairedValue(x, step);

	return -0.4*x*x + 1.4*x;
}

float GetSoftStairedValue(float x, float step, float step_soft)
{
	float d = fmod(x, step);
	float diff = step - d;
	float halfSoftStep = step_soft / 2.0;

	float e_min = (d < halfSoftStep && step_soft > 0)    * -step * (0.5 - saturate(d / step_soft));
	float e_max = (diff < halfSoftStep && step_soft > 0) * +step * (0.5 - saturate(diff / step_soft));

	x = x - d;

	return saturate(x + e_min + e_max);
}

float GetToonAttenuation(float3 worldPos, float3 light, float3 normal, float length)
{
	float ndotl, attenuation, range;

	#if defined(POINT) || defined(SPOT)
	//range = 1.0 / _LightPositionRange.w;
	// if somehow _LightPositionRange is not working we can use
	// unity_WorldToLight[0][0] which scales to make the coords
	// in order to be 0 on light and 1 on the max range.
	//range = 1.0 / unity_WorldToLight[2][2];
	range = GetLightRange(worldPos);
	#else
	range = 0.0;
	#endif

	normal.y *= 100;

	ndotl = dot(normalize(normal), light);

	return ndotl * GetLightAttenuation(float4(worldPos, 1), length);

	ndotl *= ndotl*ndotl;

	attenuation = 0.0;

	if (length < range)
	{
		float coeff = range * ndotl;

		if (coeff == 0) { return 0.0; }

		attenuation = 1.0 - saturate(length / coeff);
	}

	return attenuation;
}

float GetTerrainToonAttenuation(float3 worldPos, float3 light, float3 normal, float length)
{
	float ndotl, attenuation, range;

	normal.y *= 100;

	ndotl = dot(normalize(normal), light);

	return ndotl * GetLightAttenuation(float4(worldPos, 1), length);
}

bool CheckFragmentInsideRange(float3 worldPos)
{

	#ifdef POINT
	//range = 1.0 / _LightPositionRange.w;
	// if somehow _LightPositionRange is not working we can use
	// unity_WorldToLight[0][0] which scales to make the coords
	// in order to be 0 on light and 1 on the max range.
	//range = 1.0 / unity_WorldToLight[2][2];

	float3 light = _WorldSpaceLightPos0.xyz - worldPos.xyz;
	float range  = GetLightRange(worldPos);

	return length(light) < range;
	#endif

	return false;
}

float GetToonIntensity(float attenuation)
{
	float colorIntensity;

	if (attenuation <= 0.05) { colorIntensity = 0.0; }
	else if (attenuation > 0.05 && attenuation <= 0.1) { colorIntensity = 0.05; }
	else if (attenuation > 0.1 && attenuation <= 0.2) { colorIntensity = 0.1; }
	else if (attenuation > 0.2 && attenuation <= 0.3) { colorIntensity = 0.2; }
	else if (attenuation > 0.3 && attenuation <= 0.6) { colorIntensity = 0.5; }
	else if (attenuation > 0.6 && attenuation <= 0.9) { colorIntensity = 0.7; }
	else { colorIntensity = 1.0; }

	return colorIntensity;
}

float GetTerrainToonIntensity(float attenuation)
{
	float colorIntensity;

	if (attenuation <= 0.80) { colorIntensity = 0.0; }
	else if (attenuation > 0.80 && attenuation <= 0.81) { colorIntensity = 0.05; }
	else if (attenuation > 0.81 && attenuation <= 0.83) { colorIntensity = 0.1; }
	else if (attenuation > 0.83 && attenuation <= 0.86) { colorIntensity = 0.2; }
	else if (attenuation > 0.86 && attenuation <= 0.92) { colorIntensity = 0.4; }
	else if (attenuation > 0.92 && attenuation <= 0.96) { colorIntensity = 0.7; }
	else { colorIntensity = 1.0; }

	return colorIntensity;
}

float RimIntensity(float x)
{
	if (x < 0.005) { x = 0.0; }
	else if (x >= 0.005 && x <= 0.01) { x = 0.2; }
	else if (x > 0.01 && x <= 0.015) { x = 0.5; }
	else { x = 1.0; }

	return x;
}

float SteepedToonDiffuseIntensity(float x, float smoothness)
{
	if (x <= 0.00) { x = 0.2; }
	else if (x > 0.00 && x <= 0.15) { x = 0.5; }
	else if (x > 0.15 && x <= 0.30) { x = 0.7; }
	else { x = 1.0; }

	return x;
}

float FlatToonDiffuseIntensity(float x, float smoothness)
{
	smoothness = 0.25 * (1 - smoothness);
	x = 0.25 + 0.75*smoothstep(0.5 - smoothness, 0.5 + smoothness, x);

	return x;
}

float ToonDiffuseIntensity(float3 normal, float3 light, float attenuation, float smoothness, out float ndotl)
{
	float intensity = 1;

	ndotl = dot(normal, light);

	#if _TOONTYPE_FLAT
	intensity = FlatToonDiffuseIntensity(saturate(ndotl)*attenuation, smoothness);
	#elif _TOONTYPE_STEPPED
	intensity = SteepedToonDiffuseIntensity(saturate(ndotl)*attenuation, smoothness);
	#elif _TOONTYPE_RAMP
	intensity = 0.5 + 0.5 * ndotl;
	intensity *= attenuation;
	#endif

	ndotl *= (attenuation > 0.5);

	return intensity;
}

float SteepedToonSpecularIntensity(float x, float smoothness)
{
	return GetSoftStairedValue(saturate(4*x - 1.5), 0.25, (1 - smoothness)*0.1);
}

float RampToonSpecularIntensity(float x, float smoothness)
{
	return 0;
}

float FlatToonSpecularIntensity(float x, float smoothness)
{
	smoothness = 0.25 * (1 - smoothness);
	return smoothstep(0.5 - smoothness, 0.5 + smoothness, x);
}

float ToonSpecularIntensity(float3 normal, float3 halff, float ndotl, float power, float smoothness, out float ndoth)
{
	float intensity = 0;
	ndoth = ndotl * pow(saturate(dot(normal, halff)), power) * (power > 0);

	#if _TOONTYPE_FLAT
	intensity = FlatToonSpecularIntensity(ndoth, smoothness);
	#elif _TOONTYPE_STEPPED
	intensity = SteepedToonSpecularIntensity(ndoth, smoothness);
	#elif _TOONTYPE_RAMP
	intensity = RampToonSpecularIntensity(ndoth, smoothness);
	#endif

	return intensity;
}


half GetRimValue(float3 normalWorld, float3 viewWorld, half factor, half softness)
{

	half rim = 1.0;

	#if !_RIMTYPE_NONE

		rim = pow(saturate(dot(normalWorld, viewWorld)), factor);
		
		#if _RIMTYPE_HARD_SILHOUETTE
		rim = rim > factor;
		#endif

		#if _RIMTYPE_SOFT_SILHOUETTE
		rim *= 50 * rim;
		rim = saturate(rim);
		#endif

		#if _RIMTYPE_STEPPED
		rim = saturate(GetSoftStairedValue(7 * rim, 0.3, 0.3*softness));
		#endif

	#endif

	return rim;
}

half4 ToonDiffuse(float intensity, half4 albedo, sampler2D ramp, float attenuation)
{
	half3 diffuse;

	#if _TOONTYPE_RAMP
	diffuse = albedo.rgb * tex2D(ramp, 0.02 + intensity*0.96);
	diffuse = lerp(albedo.rgb, diffuse.rgb, 0.75);
	#else
	diffuse = albedo.rgb * intensity * attenuation;
	#endif

	return half4(diffuse, 1);
}

half4 ToonSpecular(float intensity, half4 color, float attenuation)
{
	half3 specular;

	specular = color * intensity * attenuation;

	return half4(specular, 1);
}

#endif