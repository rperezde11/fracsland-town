Shader "Custom/Toon/CelComplex"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_TintColor("Tint Albedo", Color) = (1,1,1,1)
		[NoScaleOffset]_RampTex("Ramp Texture", 2D) = "white" {}
		_Specularity("Specularity", Range(0.0, 1.0)) = 0.5
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5

		_BumpTex("Normal map", 2D) = "bump"{}
		[KeywordEnum(None, Bare, Steep, Occlussion)] _ParallaxType("Parallax", Float) = 0
		_ParallaxTex("Parallax map", 2D) = "black"{}

		[KeywordEnum(None, Flat, Stepped, Ramp)] _ToonType("Toon", Float) = 1
		[KeywordEnum(None, Outline, Silhouette)] _ContourType("Contour", Float) = 0

		_OutlineWidth("Outline Width", Range(0.0, 0.02)) = 0.0
		_OutlineColor("Outline Color", Color) = (1,1,1,1)

		[KeywordEnum(None, Rim, Hard Silhouette, Soft Silhouette, Stepped)] _RimType("Silhouette Type", Float) = 0
		_RimPower("Silhouette Strength", Range(0.0, 5.0)) = 2.0
		_RimSoftness("Silhouette Softness", Range(0.0, 1.0)) = 0.25

		_ExtraTex("Emission (R) Specular (G)", 2D) = "black" {}

		[Toggle] _Emission("Emission", Float) = 0
		_FlickerEmission("Flickering", Range(0.0, 2.0)) = 1.0
		[HDR] _EmissionColor("Emission Color", Color) = (1,1,1,1)

		[Toggle] _SpecularMap("Specular Map", Float) = 0
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma multi_compile _ SHADOWS_SCREEN

			#pragma multi_compile _TOONTYPE_NONE _TOONTYPE_FLAT _TOONTYPE_STEPPED _TOONTYPE_RAMP
			#pragma multi_compile _CONTOURTYPE_NONE _CONTOURTYPE_OUTLINE _CONTOURTYPE_SILHOUETTE
			#pragma multi_compile _RIMTYPE_NONE _RIMTYPE_RIM _RIMTYPE_HARD_SILHOUETTE _RIMTYPE_SOFT_SILHOUETTE _RIMTYPE_STEPPED
			#pragma multi_compile _PARALLAXTYPE_NONE _PARALLAXTYPE_BARE _PARALLAXTYPE_STEEP _PARALLAXTYPE_OCCLUSSION

			#pragma shader_feature _BUMPMAP
			#pragma shader_feature _PARALLAX_MAP

			#pragma shader_feature _EMISSION_ON
			#pragma shader_feature _SPECULARMAP_ON

			#pragma multi_compile_fog

			#include "../includes/Common.cginc"
			#include "../includes/UtilsBump.cginc"
			#include "../includes/UtilsToon.cginc"
			#include "UnityCG.cginc"
			#pragma vertex vertBasic
			#pragma fragment frag

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _BumpTex;
			uniform half4 _BumpTex_ST;
			uniform sampler2D _ParallaxTex;
			uniform half4 _ParallaxTex_ST;
			uniform half4 _TintColor;
			uniform sampler2D _RampTex;
			uniform half _Specularity;
			uniform half _Smoothness;
			uniform half _OutlineWidth;
			uniform half4 _OutlineColor;
			uniform half _RimPower;
			uniform half _RimSoftness;
			uniform sampler2D _ExtraTex;
			uniform half4 _ExtraTex_ST;
			uniform half4 _EmissionColor;
			uniform float _FlickerEmission;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float shadowAttenuation;
				float ndotl, ndoth;
				float diffuseIntensity, specularIntensity, rimIntensity;
				float3 normal, light, view, halff;
				float4 diffuse, specular, rim, emission, final;
				half4 texMain;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);

				half2 extra = half2(0,1);

				#if USE_EXTRA_MAP
				extra = tex2D(_ExtraTex, i.texcoords.xy * _ExtraTex_ST.xy + _ExtraTex_ST.zw).xy;
				#endif

				#if _PARALLAX_MAP
				CalculateParallaxTexcoords(i.viewSurface, (1-_Smoothness) * 0.05, 30, i.texcoords, _ParallaxTex, _ParallaxTex_ST);
				#endif

				normal = CalculateBumpNormals(1 - _Smoothness, i, _BumpTex, _BumpTex_ST);

				texMain = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				diffuseIntensity = ToonDiffuseIntensity(normal, light, SHADOW_ATTENUATION(i), _Smoothness, ndotl);
				specularIntensity = ToonSpecularIntensity(normal, halff, ndotl, _Specularity * 500, _Smoothness, ndoth);

				#if _CONTOURTYPE_SILHOUETTE
				rimIntensity = GetRimValue(normal, view, _RimPower, _RimSoftness);
				#else
				rimIntensity = 1;
				#endif

				diffuse = ToonDiffuse(diffuseIntensity, texMain * _LightColor0 * _TintColor, _RampTex, 1);
				specular = ToonSpecular(specularIntensity, _LightColor0, 1);
				rim = rimIntensity;

				#if _SPECULARMAP_ON
				specular *= extra.g;
				#endif

				emission = 0;

				#if _EMISSION_ON
				if(_FlickerEmission > 0)
				{
					float speed = _Time.w % (_FlickerEmission * _FlickerEmission * 2);
					speed = saturate(-abs((1.0 / _FlickerEmission) * speed - _FlickerEmission) + _FlickerEmission);
					emission = _EmissionColor * extra.r * speed;
				}
				#endif

				final = (diffuse + specular + emission) * rim;

				AddFog(final, i.posWorld.w);

				return final;
			}

			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ForwardAdd" }

			Blend One One

			CGPROGRAM

			#pragma multi_compile_fwdadd_fullshadows

			#pragma multi_compile _TOONTYPE_NONE _TOONTYPE_FLAT _TOONTYPE_STEPPED _TOONTYPE_RAMP
			#pragma multi_compile _CONTOURTYPE_NONE _CONTOURTYPE_OUTLINE _CONTOURTYPE_SILHOUETTE
			#pragma multi_compile _RIMTYPE_NONE _RIMTYPE_RIM _RIMTYPE_HARD_SILHOUETTE _RIMTYPE_SOFT_SILHOUETTE _RIMTYPE_STEPPED
			#pragma multi_compile _PARALLAXTYPE_NONE _PARALLAXTYPE_BARE _PARALLAXTYPE_STEEP _PARALLAXTYPE_OCCLUSSION

			#pragma multi_compile _BUMPMAP
			#pragma multi_compile _PARALLAX_MAP

			#pragma shader_feature _SPECULARMAP_ON

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../includes/Common.cginc"
			#include "../includes/UtilsBump.cginc"
			#include "../includes/UtilsToon.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform half4 _TintColor;
			uniform sampler2D _BumpTex;
			uniform half4 _BumpTex_ST;
			uniform sampler2D _ParallaxTex;
			uniform half4 _ParallaxTex_ST;
			uniform sampler2D _RampTex;
			uniform half _Specularity;
			uniform half _Smoothness;
			uniform half _OutlineWidth;
			uniform half4 _OutlineColor;
			uniform half _RimPower;
			uniform half _RimSoftness;
			uniform sampler2D _ExtraTex;
			uniform half4 _ExtraTex_ST;
			uniform half4 _EmissionColor;
			uniform float _FlickerEmission;

			half4 frag(v2f_basic i) : COLOR
			{
				// In case some directional light tries to add
				// color on the forward add pass...
				if (_WorldSpaceLightPos0.w == 0) { return 0; }

				float attenuation;
				float ndotl, ndoth;
				float diffuseIntensity, specularIntensity, rimIntensity;
				float3 normal, light, view, halff;
				float4 diffuse, specular, rim, final;
				half4 texMain;

				half2 extra = half2(0, 1);

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);

				#if _PARALLAX_MAP
				CalculateParallaxTexcoords(i.viewSurface, (1-_Smoothness) * 0.01, 10, i.texcoords, _ParallaxTex, _ParallaxTex_ST);
				#endif
				
				normal = CalculateBumpNormals(1 - _Smoothness, i, _BumpTex, _BumpTex_ST);

				texMain = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				#if USE_EXTRA_MAP
				extra = tex2D(_ExtraTex, i.texcoords.xy * _ExtraTex_ST.xy + _ExtraTex_ST.zw).xy;
				#endif

				diffuseIntensity = ToonDiffuseIntensity(normal, light, SHADOW_ATTENUATION(i), _Smoothness, ndotl) * 3 * attenuation;
				specularIntensity = ToonSpecularIntensity(normal, halff, ndotl, _Specularity * 500, _Smoothness, ndoth);
				
				#if _CONTOURTYPE_SILHOUETTE
				rimIntensity = GetRimValue(normal, view, _RimPower, _RimSoftness);
				#else
				rimIntensity = 1;
				#endif

				diffuse = ToonDiffuse(diffuseIntensity, texMain * _LightColor0 * _TintColor, _RampTex, 1);
				specular = ToonSpecular(specularIntensity, _LightColor0, 1);
				rim = rimIntensity;

				#if _SPECULARMAP_ON
				specular *= extra.g;
				#endif

				final = (diffuse + specular) * rim;

				return final;
			}

			ENDCG
		}

		UsePass "CommonPass/SHADOWCASTER"
		//UsePass "CommonPass/OUTLINE"
	}

	CustomEditor "CelShaderComplexGUI"
}