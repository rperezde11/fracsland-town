Shader "Custom/toon/simple-emit"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_EmitTex("Emission (A)", 2D) = "black" {}
		_EmitColor("Emission Color", Color) = (1,1,1,1)
		_Blending("Emission Blending", Range(0.0, 1.0)) = 0.7

		_Outline("Outline Width", Range(0.0, 0.05)) = 0.01

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
			#pragma multi_compile _ FLICKER_ENABLED

			#pragma shader_feature _RIMTYPE_NONE _RIMTYPE_RIM _RIMTYPE_HARD_SILHOUETTE _RIMTYPE_SOFT_SILHOUETTE _RIMTYPE_HARD _RIMTYPE_SOFT

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../../includes/Common.cginc"
			#include "../../includes/UtilsToon.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _EmitTex;
			uniform half4 _EmitTex_ST;
			uniform half4 _EmitColor;
			uniform half _Blending;
			uniform half _FlickerSpeed;
			uniform half _RimPower;

			half4 frag(v2f_basic i) : COLOR
			{	
				float attenuation;
				float3 normal, light, view;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, view, attenuation);

				half4 tex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 emitTex = tex2D(_EmitTex, i.texcoords.xy * _EmitTex_ST.xy + _EmitTex_ST.zw);
				
				half rim = GetRimValue(normal, view, _RimPower, 0.01);

				// Color
				float colorIntensity = 0;//ToonShadingIntensity(saturate(dot(normal, light)) * SHADOW_ATTENUATION(i));
				half4 finalLight = colorIntensity * half4(_LightColor0.rgb * tex.rgb, 1.0);

				float flicker = 1.0;

				#if FLICKER_ENABLED
				flicker = abs((_SinTime.w + _CosTime.w) * sin(_FlickerSpeed * _Time.y));
				#endif

				finalLight += lerp(finalLight, flicker * emitTex.a * _EmitColor, _Blending);

				rim  = 1;

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

			#pragma shader_feature _RIMTYPE_NONE _RIMTYPE_RIM _RIMTYPE_HARD_SILHOUETTE _RIMTYPE_SOFT_SILHOUETTE _RIMTYPE_HARD _RIMTYPE_SOFT

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../../includes/Common.cginc"
			#include "../../includes/UtilsToon.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform float _RimPower;

			half4 frag(v2f_basic i) : COLOR
			{
				// In case some directional light tries to add
				// color on the forward add pass...
				if(_WorldSpaceLightPos0.w == 0) { return 0; }

				float attenuation;
				float3 normal, light, view;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, view, attenuation);

				half4 tex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				half rim = GetRimValue(normal, view, _RimPower, 0.01);

				// Color
				float colorIntensity = 0;//ToonShadingIntensity(saturate(dot(normal, light)) * SHADOW_ATTENUATION(i)) * 3 * attenuation;

				half3 finalLight = colorIntensity * _LightColor0.rgb * tex.rgb;

				rim = 1;

				return rim * half4(finalLight, 1);
			}

			ENDCG
		}
	
		UsePass "CommonPass/SHADOWCASTER"
	}
}