Shader "Custom/Reflections/Complex"
{
	Properties
	{
		_MainTex("Albedo Texture", 2D) = "white"{}
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)

		_BumpTex("Normal map", 2D) = "bump"{}

		_Metallic("Metallic", Range(0.001, 1.0)) = 0.001
		_Smoothness("Smoothness", Range(0.001, 1.0)) = 1.0

		_ExtraTex("(R) Metallic (G) Smoothness", 2D) = "white"{}

		[Toggle(REFLECTION_OPTIMIZE)] _Optimize("Optimize Reflection", Float) = 1.0
		[KeywordEnum(None,Cube,Aproximation,Perfect)] _Reflection("Spherical Reflection Type", Float) = 1
	}

	SubShader
	{
		CGINCLUDE
		
		#include "../includes/Common.cginc"
		#include "../includes/UtilsLight.cginc"

		#pragma multi_compile_fog
		#pragma multi_compile _ EXTRA_MAP
		#pragma multi_compile _ _BUMPMAP

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform half4 _Color;
		
		uniform float _Metallic;
		uniform float _Smoothness;

		#if _BUMPMAP
		uniform sampler2D _BumpTex;
		uniform float4 _BumpTex_ST;
		#endif

		#if EXTRA_MAP
		uniform sampler2D _ExtraTex;
		uniform float4 _ExtraTex_ST;
		#endif

		ENDCG

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma shader_feature _ REFLECTION_OPTIMIZE
			#pragma shader_feature _REFLECTION_NONE _REFLECTION_CUBE _REFLECTION_APROXIMATION _REFLECTION_PERFECT

			#pragma vertex vertBasic
			#pragma fragment frag

			half4 frag(v2f_basic i) : COLOR
			{	
				float attenuation;
				float ndotl;
				float3 light, view, halff;
				half3 specular, diffuse, reflection;
				half3 albedo, specTint;
				half4 texAlbedo, texN, texExtra;
				half metallic, oneMinusReflectivity, smoothness, roughness;

				albedo = _Color.rgb * tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				#if EXTRA_MAP
					texExtra = tex2D(_ExtraTex, i.texcoords * _ExtraTex_ST.xy + _ExtraTex_ST.zw);
					metallic = texExtra.r;
					smoothness = texExtra.g;
				#else
					metallic = _Metallic;
					smoothness = _Smoothness;
				#endif

				roughness = 1 - smoothness;
				oneMinusReflectivity = 1 - metallic;

				specular = 0;
				diffuse = 0;
				reflection = 0;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);

				#if _BUMPMAP
				texN = tex2D(_BumpTex, i.texcoords * _BumpTex_ST.xy + _BumpTex_ST.zw);
				i.normalWorld = BUMP_NORMAL(i, texN, roughness);
				#endif

				attenuation *= SHADOW_ATTENUATION(i);

				ndotl = saturate(dot(i.normalWorld, light)) * attenuation;
				
				specTint = lerp(albedo.rgb, 1, oneMinusReflectivity);

				diffuse = albedo.rgb * (0.5 + 0.5 * ndotl);

				reflection = SampleReflectionFragmentShader(i, view, roughness);
				reflection = lerp(diffuse, specTint * reflection, 0.25*smoothness+0.75*metallic);

				specular = ndotl * GetSpecularColorByMetallic(albedo.rgb, oneMinusReflectivity, i.normalWorld, halff, roughness);

				return half4(reflection + specular, 1);
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

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float ndotl;
				float3 light, view, halff;
				half3 specular, diffuse, reflection;
				half3 albedo, specTint;
				half4 texAlbedo, texN, texExtra;
				half metallic, oneMinusReflectivity, smoothness, roughness;

				albedo = _Color.rgb * tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				#if EXTRA_MAP
					texExtra = tex2D(_ExtraTex, i.texcoords * _ExtraTex_ST.xy + _ExtraTex_ST.zw);
					metallic = texExtra.r;
					smoothness = texExtra.g;
				#else
					metallic = _Metallic;
					smoothness = _Smoothness;
				#endif

				roughness = 1 - smoothness;
				oneMinusReflectivity = 1 - metallic;

				specular = 0;
				diffuse = 0;
				reflection = 0;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);

				#if _BUMPMAP
				texN = tex2D(_BumpTex, i.texcoords * _BumpTex_ST.xy + _BumpTex_ST.zw);
				i.normalWorld = BUMP_NORMAL(i, texN, roughness);
				#endif

				attenuation *= SHADOW_ATTENUATION(i);

				ndotl = saturate(dot(i.normalWorld, light)) * attenuation;

				specTint = lerp(albedo.rgb, 1, oneMinusReflectivity);

				diffuse = albedo.rgb * (0.5 + 0.5 * ndotl);

				reflection = SampleReflectionFragmentShader(i, view, roughness);
				reflection = lerp(diffuse, specTint * reflection, 0.25*smoothness + 0.75*metallic);

				specular = ndotl * GetSpecularColorByMetallic(albedo.rgb, oneMinusReflectivity, i.normalWorld, halff, roughness);

				return half4((reflection + specular ) * attenuation, 1);
			}

			ENDCG
		}

		UsePass "CommonPass/SHADOWCASTER"
	}

	CustomEditor "ReflectiveShaderGUI"
}