#ifndef UTILS_REFLECTIVITY_INCLUDE
#define UTILS_REFLECTIVITY_INCLUDE

/****************************************************
Given a value between 0 and 1 returns a value between
0 and peak. The returning value describes a quadratic
formula that begins and ends at zero.

x = 0.0 => 0.0
x = 0.5 => peak
x = 1.0 => 0.0
****************************************************/

float GetSmoothedValue(float value, float peak)
{
	value = saturate(value);

	return -peak * 4 * value * (value - 1);
}

float3 GetSmoothedValues(float3 values, float peak)
{
	values.x = GetSmoothedValue(values.x, peak);
	values.y = GetSmoothedValue(values.y, peak);
	values.z = GetSmoothedValue(values.z, peak);

	return values;
}

/******************************************************************
Gets the nearest point on the surface of the sphere that covers
the spec reflection box. This eliminates the box-like projections.
*******************************************************************/
float3 GetSphereVector(float3 position, float3 direction, float maxLength, float3 sphereCenter, float radius)
{
	float3 diff = direction * maxLength;
	float3 centerToPosition = position - sphereCenter;

	// (x1−x0)^2+(y1−y0)^2 ···
	float a = dot(diff, diff);
	// 2(x1−x0)(x0−h)+2(y1−y0)(y0−k) ···
	float b = 2 * dot(diff, centerToPosition);
	// (x0−h)^2+(y0−k)^2 ··· - r^2
	float c = dot(centerToPosition, centerToPosition) - radius*radius;

	float t = (-b + sqrt(b*b - 4 * a*c)) / (2 * a);

	return direction * maxLength * t;
}

/*********************************************************
Computes the new direction of the vector depending on the 
position of the object in the probe.
**********************************************************/
float3 ComputeReflexDirection(float3 view, float3 normal, float3 wPos)
{
	float3 reflection = reflect(-view, normal);
	float3 oldReflection = reflection;
	
	if (unity_SpecCube0_ProbePosition.w > 0)
	{
		float3 scalar = ((reflection > 0 ? unity_SpecCube0_BoxMax.xyz : unity_SpecCube0_BoxMin.xyz) - wPos) / reflection;
		float nearestBoxPlane = min(scalar.x, min(scalar.y, scalar.z));

		float3 b_size = (unity_SpecCube0_BoxMax - unity_SpecCube0_BoxMin) * 0.5;

		#if _REFLECTION_CUBE
		
			reflection = nearestBoxPlane * reflection + wPos - unity_SpecCube0_ProbePosition.xyz;
		
		#elif _REFLECTION_PERFECT
				
			float s_radius = length(b_size);
			float3 vec = GetSphereVector(wPos, reflection, 300, unity_SpecCube0_ProbePosition.xyz, s_radius);
				
			reflection = vec + wPos - unity_SpecCube0_ProbePosition.xyz;

		#elif _REFLECTION_APROXIMATION
			
			float s_radius = length(b_size);
			float b_radius = max(b_size.z, max(b_size.x, b_size.y));
			float3 boxPosition = wPos + nearestBoxPlane * reflection;
			float3 b_surfaceDiff = (unity_SpecCube0_BoxMax - unity_SpecCube0_ProbePosition.xyz) / (b_radius * 2);
			float factor = length(GetSmoothedValues(b_surfaceDiff, s_radius - b_radius));

			reflection = (nearestBoxPlane + factor) * reflection + wPos - unity_SpecCube0_ProbePosition.xyz;
		#endif
	}

	return normalize(lerp(reflection, oldReflection, 0.5));
}

float3 ComputeRefractDirection(float3 view, float3 normal, float refractiveIndexFactor, float3 wPos)
{
	float3 refraction = refract(-view, normal, refractiveIndexFactor);
	float3 old_refraction = refraction;

	if (unity_SpecCube0_ProbePosition.w > 0)
	{
		float3 scalar = ((refraction > 0 ? unity_SpecCube0_BoxMax.xyz : unity_SpecCube0_BoxMin.xyz) - wPos) / refraction;
		float nearestBoxPlane = min(scalar.x, min(scalar.y, scalar.z));

		float3 b_size = (unity_SpecCube0_BoxMax - unity_SpecCube0_BoxMin) * 0.5;

		#if _REFRACTION_CUBE

			refraction = nearestBoxPlane * refraction + wPos - unity_SpecCube0_ProbePosition.xyz;

		#elif _REFRACTION_PERFECT

			float s_radius = length(b_size);
			float3 vec = GetSphereVector(wPos, refraction, 300, unity_SpecCube0_ProbePosition.xyz, s_radius);

			refraction = vec + wPos - unity_SpecCube0_ProbePosition.xyz;

		#elif _REFRACTION_APROXIMATION

			float s_radius = length(b_size);
			float b_radius = max(b_size.z, max(b_size.x, b_size.y));
			float3 boxPosition = wPos + nearestBoxPlane * refraction;
			float3 b_surfaceDiff = (unity_SpecCube0_BoxMax - unity_SpecCube0_ProbePosition.xyz) / (b_radius * 2);
			float factor = length(GetSmoothedValues(b_surfaceDiff, s_radius - b_radius));

			refraction = (nearestBoxPlane + factor) * refraction + wPos - unity_SpecCube0_ProbePosition.xyz;
		#endif
	}

	return normalize(lerp(refraction, old_refraction, 0.55));
}

half4 SampleLightProbeCubemap(float3 coords, float roughness)
{
	return UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, coords, roughness * 6);
}


half4 SampleReflectionFragmentShader(v2f_basic i, float3 view, float roughness)
{
	float3 reflection = 0;
	half4 reflectionColor = 0;

	#if REFLECTION_ON || !REFLECTION_NONE
		
		#if REFLECTION_OPTIMIZE
		
			reflection = normalize(i.reflection);

			#if _BUMPMAP
			reflection = 0.5 * (reflection + i.normalWorld);
			#endif
		
		#else	
		
			reflection = ComputeReflexDirection(view, i.normalWorld, i.posWorld);
		
		#endif

		reflectionColor = SampleLightProbeCubemap(reflection, roughness);
	
	#endif

	return reflectionColor;
}

float ComputeSchlicksReflectance(float n1, float n2, float3 viewWorld, float3 normalWorld)
{
	float reflectance = 0;

	#if REFLECTION_ON
		float cos_theta = 1 - dot(viewWorld, normalWorld);
		float cos_theta_5 = cos_theta * cos_theta;

		cos_theta_5 *= cos_theta_5;
		cos_theta_5 *= cos_theta;

		float r0 = (n1 - n2) / (n1 + n2);
		r0 = r0*r0;

		reflectance = saturate(r0 + (1 - r0) * cos_theta_5);
	#endif

	return reflectance;
}

#endif