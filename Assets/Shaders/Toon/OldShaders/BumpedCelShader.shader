Shader "Custom/toon/CelBumped"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_TintColor("Tint Albedo", Color) = (1,1,1,1)
		[NoScaleOffset]_RampTex("Ramp Texture", 2D) = "white" {}
		_Specularity("Specularity", Range(0.0, 1.0)) = 0.5
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5

		[KeywordEnum(None, Flat, Stepped, Ramp)] _ToonType("Toon", Float) = 1
		[KeywordEnum(None, Outline, Silhouette)] _ContourType("Contour", Float) = 1

		_OutlineWidth("Outline Width", Range(0.0, 0.02)) = 0.0
		_OutlineColor("Outline Color", Color) = (1,1,1,1)

		[KeywordEnum(None, Rim, Hard Silhouette, Soft Silhouette, Hard, Soft)] _RimType("Silhouette Type", Float) = 3
		_RimPower("Silhouette Strength", Range(0.0, 5.0)) = 2.0
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
			#pragma multi_compile _RIMTYPE_NONE _RIMTYPE_RIM _RIMTYPE_HARD_SILHOUETTE _RIMTYPE_SOFT_SILHOUETTE _RIMTYPE_HARD _RIMTYPE_SOFT

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../includes/Common.cginc"
			#include "../includes/UtilsBump.cginc"
			#include "../includes/UtilsToon.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform half4 _TintColor;
			uniform sampler2D _RampTex;
			uniform half4 _RampTex_ST;
			uniform half _Specularity;
			uniform half _Smoothness;
			uniform half _OutlineWidth;
			uniform half4 _OutlineColor;
			uniform half _RimPower;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float ndotl, ndoth;
				float diffuseIntensity, specularIntensity, rimIntensity;
				float3 normal, light, view, halff;
				float4 diffuse, specular, rim, final;
				half4 texMain;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, view, halff, attenuation);

				texMain = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				diffuseIntensity = ToonDiffuseIntensity(normal, light, SHADOW_ATTENUATION(i), _Smoothness, ndotl);
				specularIntensity = ToonSpecularIntensity(normal, halff, ndotl, _Specularity * 500, _Smoothness, ndoth);
				rimIntensity = GetRimValue(normal, view, _RimPower);

				diffuse = ToonDiffuse(diffuseIntensity, texMain * _LightColor0 * _TintColor, _RampTex, 1);
				specular = ToonSpecular(specularIntensity, _LightColor0, 1);
				rim = rimIntensity;

				final = (diffuse + specular) * rim;

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
			#pragma multi_compile _RIMTYPE_NONE _RIMTYPE_RIM _RIMTYPE_HARD_SILHOUETTE _RIMTYPE_SOFT_SILHOUETTE _RIMTYPE_HARD _RIMTYPE_SOFT

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../includes/Common.cginc"
			#include "../includes/UtilsBump.cginc"
			#include "../includes/UtilsToon.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform half4 _TintColor;
			uniform sampler2D _RampTex;
			uniform half4 _RampTex_ST;
			uniform half _Specularity;
			uniform half _Smoothness;
			uniform half _OutlineWidth;
			uniform half4 _OutlineColor;
			uniform half _RimPower;

			uniform half4 _LightColor0;

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

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, view, halff, attenuation);

				texMain = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				diffuseIntensity = ToonDiffuseIntensity(normal, light, SHADOW_ATTENUATION(i), _Smoothness, ndotl) * 3 * attenuation;
				specularIntensity = ToonSpecularIntensity(normal, halff, ndotl, _Specularity * 500, _Smoothness, ndoth);
				rimIntensity = GetRimValue(normal, view, _RimPower);

				diffuse = ToonDiffuse(diffuseIntensity, texMain * _LightColor0 * _TintColor, _RampTex, 1);
				specular = ToonSpecular(specularIntensity, _LightColor0, 1);
				rim = rimIntensity;

				final = (diffuse + specular) * rim;

				return final;
			}

			ENDCG
		}

		UsePass "CommonPass/SHADOWCASTER"
		UsePass "CommonPass/OUTLINE"
	}

		CustomEditor "CelShaderBumpedGUI"
}