Shader "Custom/toon/simple"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		
		_Outline("Outline Width", Range(0.0, 0.1)) = 0.1

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
			#pragma multi_compile _ RIM_ON

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
				float attenuation;
				float3 normal, light, view;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, attenuation);

				half4 tex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				half rim = GetRimValue(i.normalWorld, view, _RimPower, 0.01);

				// Color
				float colorIntensity = 0;//ToonShadingIntensity(saturate(dot(i.normalWorld, light)) * SHADOW_ATTENUATION(i));

				half4 finalLight = colorIntensity * half4(_LightColor0.rgb * tex.rgb, 1.0);

				rim = 1;

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
			#pragma multi_compile _ RIM_ON

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
				float attenuation;
				float3 normal, light, view;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, attenuation);

				half4 tex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				
				half rim = GetRimValue(i.normalWorld, view, _RimPower, 0.01);

				// Color
				float colorIntensity = 0;//ToonShadingIntensity(saturate(dot(i.normalWorld, light)) * SHADOW_ATTENUATION(i)) * 3 *	attenuation;
				half4 finalLight = colorIntensity * half4(_LightColor0.rgb * tex.rgb, 1.0);
				
				rim = 1;

				return rim * finalLight;
			}

			ENDCG
		}
	
		UsePass "CommonPass/SHADOWCASTER"
	}
}