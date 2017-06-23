#ifndef BUMP_COMMON_INCLUDE
#define BUMP_COMMON_INCLUDE

#include "UnityCG.cginc"

#define NO_PARALLAX _PARALLAXTYPE_NONE
#define SIMPLE_PARALLAX _PARALLAXTYPE_BARE
#define COMPLEX_PARALLAX _PARALLAXTYPE_STEEP || _PARALLAXTYPE_OCCLUSSION

#define BUMP_COORDS(idx1, idx2) float3 _TangentWorld : TEXCOORD##idx1; float3 _BinormalWorld : TEXCOORD##idx2;

#define TRANSFER_BUMP(a) \
a._TangentWorld = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0))).xyz; \
a._BinormalWorld = normalize(cross(a.normalWorld, a._TangentWorld) * v.tangent.w);

#define BUMP_NORMAL(input, sampledN, bumpiness) GetBumpMappedNormal(sampledN, input.normalWorld, input._TangentWorld, input._BinormalWorld, bumpiness)

struct ParallaxInfo
{
	float3 view;
	float heightFactor;

	#if COMPLEX_PARALLAX
	float minLayers;
	float maxLayers;
	#endif
};


inline half3 GetBumpMappedNormal(half4 texN, float3 normal, float3 tangent, float3 binormal, float bumpiness)
{
	// Unpacking normals
	float3 localCoords = float3(2.0 * texN.ag - 1.0, 0.0);

	localCoords.xy *= bumpiness;
	localCoords.z = 1 - 0.5 * dot(localCoords.xy, localCoords.xy);

	// Construct the matrix to pass from local space to
	// world space
	half3x3 localToWorldTranspose = half3x3(
		normalize(tangent),
		normalize(binormal),
		normalize(normal)
	);

	return normalize(mul(localCoords, localToWorldTranspose));
}

float2 GetParallaxSteepUV(int numSteps, float3 viewDir, float2 texcoords, float hfactor, float maxLayers, sampler2D tex, float4 tex_ST)
{
	numSteps = 12;
	maxLayers = 50;

	float dHeight = 1.0 / numSteps;
	float2 dDisplacement = (hfactor * viewDir.xy / viewDir.z) / numSteps;
	
	float counter = 0;
	float weight = 0.5;

	float2 currentCoord = texcoords.xy;
	float currentHeight = tex2D(tex, currentCoord * tex_ST.xy + tex_ST.zw).x;
	float layerHeight = 1.0;

	
	while (currentHeight <= layerHeight && counter <= numSteps && counter < maxLayers)
	{
		currentCoord -= dDisplacement;

		currentHeight = tex2D(tex, currentCoord * tex_ST.xy + tex_ST.zw).x;
		layerHeight -= dHeight;

		counter++;
	}

	float2 prevCoords = currentCoord + dDisplacement;

	#if _PARALLAXTYPE_OCCLUSSION
	float heightDiffAfter = currentHeight - layerHeight;
	float heightDiffBefore = tex2D(tex, prevCoords * tex_ST.xy + tex_ST.zw).x - (layerHeight + dHeight);

	weight = saturate(heightDiffAfter / (heightDiffAfter - heightDiffBefore));
	#endif

	return lerp(currentCoord, prevCoords, weight);
}

#endif