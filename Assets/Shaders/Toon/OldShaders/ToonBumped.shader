Shader "Custom/toon/bumped-emit"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BumpTex("Normal Map", 2D) = "bump" {}
		_EmitTex("Emission (A)", 2D) = "black" {}
		_EmitColor("Emission Color", Color) = (1,1,1,1)
		_Blending("Emission Blending", Range(0.0, 1.0)) = 0.7
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5

		[Toggle(FLICKER_ENABLED)] _Flicker("Flicker Lights", Float) = 1
		_FlickerSpeed("_Flicker Speed", Range(1.0, 15.0)) = 10.0

		[KeywordEnum(None, Rim, Hard Silhouette, Soft Silhouette, Hard, Soft)] _RimType("Rim Type", Float) = 3
		_RimPower("Silhouette Strength", Range(0.0, 5.0)) = 2.0
	}

	SubShader
	{

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile _BUMPMAP
			#pragma multi_compile _ FLICKER_ENABLED

			#pragma shader_feature _RIMTYPE_NONE _RIMTYPE_RIM _RIMTYPE_HARD_SILHOUETTE _RIMTYPE_SOFT_SILHOUETTE _RIMTYPE_HARD _RIMTYPE_SOFT

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../../includes/Common.cginc"
			#include "../../includes/UtilsBump.cginc"
			#include "../../includes/UtilsToon.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _BumpTex;
			uniform half4 _BumpTex_ST;
			uniform sampler2D _EmitTex;
			uniform half4 _EmitTex_ST;
			uniform half4 _EmitColor;
			uniform half _Blending;
			uniform half _FlickerSpeed;
			uniform half _RimPower;
			uniform float _Smoothness;

			half4 frag(v2f_basic i) : COLOR
			{
				return 0;
				float attenuation;
				float3 normal, light, view;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, view, attenuation);

				half4 tex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 texN = tex2D(_BumpTex, i.texcoords.xy * _BumpTex_ST.xy + _BumpTex_ST.zw);
				half4 emitTex = tex2D(_EmitTex, i.texcoords.xy * _EmitTex_ST.xy + _EmitTex_ST.zw);

				normal = BUMP_NORMAL(i, texN, 2 * (1 - _Smoothness));
				half rim = GetRimValue(normal, view, _RimPower, 0.01);

				// Color
				float colorIntensity = 0;//ToonShadingIntensity(saturate(dot(normal, light)) * SHADOW_ATTENUATION(i));

				float ndotl = saturate(dot(normal, light));
				float3 halff = normalize(light+view);
				float ndoth = ndotl * saturate(pow(dot(normal, halff), 100));
				
				colorIntensity = ndotl > 0.1 ? 0.7 : 0.3;
				float specularIntensity = ndoth > 0.25;

				half4 finalLight = (colorIntensity + specularIntensity) * half4(_LightColor0.rgb * tex.rgb, 1.0);

				float flicker = 1.0;

				#if FLICKER_ENABLED
				flicker = abs((_SinTime.w + _CosTime.w) * sin(_FlickerSpeed * _Time.y));
				#endif

				finalLight += lerp(finalLight, flicker * emitTex.a * _EmitColor, _Blending);

				return rim * finalLight;
			}

			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ForwardAdd" }

			Blend One One

			CGPROGRAM

			#pragma multi_compile_fwdadd_fullshadows

			#pragma multi_compile _BUMPMAP
			#pragma shader_feature _RIMTYPE_NONE _RIMTYPE_RIM _RIMTYPE_HARD_SILHOUETTE _RIMTYPE_SOFT_SILHOUETTE _RIMTYPE_HARD _RIMTYPE_SOFT

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../../includes/Common.cginc"
			#include "../../includes/UtilsBump.cginc"
			#include "../../includes/UtilsToon.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _BumpTex;
			uniform half4 _BumpTex_ST;
			uniform float _RimPower;
			uniform float _Smoothness;

			half4 frag(v2f_basic i) : COLOR
			{
				// In case some directional light tries to add
				// color on the forward add pass...
				if (_WorldSpaceLightPos0.w == 0) { return 0; }

				float attenuation;
				float3 normal, light, view;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, view, attenuation);

				half4 tex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 texN = tex2D(_BumpTex, i.texcoords.xy * _BumpTex_ST.xy + _BumpTex_ST.zw);

				normal = BUMP_NORMAL(i, texN, 2 * (1 - _Smoothness));
				half rim = GetRimValue(normal, view, _RimPower, 0.01);

				// Color
				float colorIntensity = 0;//ToonShadingIntensity(saturate(dot(normal, light)) * SHADOW_ATTENUATION(i)) * 3 * attenuation;

				float ndotl = saturate(dot(normal, light));
				float3 halff = normalize(light + view);
				float ndoth = ndotl * saturate(pow(dot(normal, halff), 100));

				colorIntensity = ndotl > 0.1 ? 0.7 : 0.3;
				float specularIntensity = ndoth > 0.5;

				//return colorIntensity * attenuation;// + specularIntensity;

				half3 finalLight = (attenuation>0.001) * (colorIntensity + specularIntensity) * _LightColor0.rgb * tex.rgb;

				return rim * half4(finalLight, 1);
			}

			ENDCG
		}

		UsePass "CommonPass/SHADOWCASTER"
	}

}