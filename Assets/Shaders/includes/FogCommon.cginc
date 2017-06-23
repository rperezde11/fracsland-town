#ifndef FOG_COMMON_INCLUDE
#define FOG_COMMON_INCLUDE

#include "UnityCG.cginc"

#if (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))
	#define FOG_ACTIVE 1
#endif


float GetFogFactor(float3 clipPos)
{
	// clipPos.z esta entre 0 y 1, donde 0 es el plano far
	// y 1 el plano near

	float fogFactor = 0;

	#if FOG_ACTIVE

		// UNITY_Z_0_FAR_FROM_CLIPSPACE calcula la profundidad REAL donde
		// clipPos.z = 0 ==> posicion camara
		// clipPos.z = 1 ==> posicion plano far

		// UNITY_CALC_FOG_FACTOR_RAW calcula el fog factor en funcion del
		// modo de fog activo y lo guarda en la variable unityFogFactor

		UNITY_CALC_FOG_FACTOR_RAW(UNITY_Z_0_FAR_FROM_CLIPSPACE(clipPos.z));
		fogFactor = unityFogFactor;
	#endif

	return fogFactor;
}

float GetFogFactor(float4 clipPos)
{
	return GetFogFactor(clipPos.xyz);
}

void AddFog(inout half4 color, float fogFactor)
{
	#if FOG_ACTIVE
		color.rgb = lerp(unity_FogColor.rgb, color.rgb, saturate(fogFactor));
	#endif
}

#endif