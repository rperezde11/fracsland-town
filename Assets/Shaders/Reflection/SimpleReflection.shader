Shader "Custom/Reflections/Simple"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Smoothness("Smoothness", Range(0,1)) = 0.1

		[Toggle(REFLECTION_OPTIMIZE)] _Optimize("Optimize Reflection", Float) = 1.0
		[KeywordEnum(None,Cube,Aproximation,Perfect)] _Reflection("Spherical Reflection Type", Float) = 1
	}

		SubShader
		{
			Pass
			{
				Tags{ "LightMode" = "ForwardBase" }

				CGPROGRAM

				#pragma multi_compile _ SHADOWS_SCREEN
				#pragma shader_feature _ REFLECTION_OPTIMIZE
				#pragma shader_feature _REFLECTION_NONE _REFLECTION_CUBE _REFLECTION_APROXIMATION _REFLECTION_PERFECT

				#pragma vertex vertBasic
				#pragma fragment frag
					
				#include "../includes/Common.cginc"

				uniform half4 _Color;
				uniform float _Smoothness;

				uniform float4 _LightColor0;

				half3 frag(v2f_basic i) : COLOR
				{
					float attenuation;
					float3 light, view, halff;
					half3 diffuse, reflection;
					half roughness;

					InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);
					attenuation *= SHADOW_ATTENUATION(i);

					roughness = 1 - _Smoothness;

					diffuse = _Color.rgb * (0.5 + 0.5 * dot(light, i.normalWorld) * attenuation);
					reflection = SampleReflectionFragmentShader(i, view, roughness);

					return lerp(reflection, diffuse, roughness);
				}

				ENDCG
			}

			Pass
			{
				Tags{ "LightMode" = "ForwardAdd" }

				Blend One One

				CGPROGRAM

				#pragma multi_compile_fwdadd_fullshadows

				#pragma vertex vertBasic
				#pragma fragment frag

				#include "../includes/Common.cginc"

				uniform float _Smoothness;
				uniform half4 _Color;

				uniform float4 _LightColor0;

				half3 frag(v2f_basic i) : COLOR
				{
					float attenuation;
					float3 light, view, halff;
					half3 diffuse, reflection;
					half roughness;

					InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);
					attenuation *= SHADOW_ATTENUATION(i);

					roughness = 1 - _Smoothness;

					diffuse = _Color.rgb * (0.5 + 0.5 * dot(light, i.normalWorld) * attenuation);
					reflection = SampleReflectionFragmentShader(i, view, roughness);

					return lerp(reflection, diffuse, roughness);
				}

			ENDCG
		}

		UsePass "CommonPass/SHADOWCASTER"
	}
}