Shader "Custom/bumped/simple"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_BumpTex("Normal Texture", 2D) = "bump" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Smoothness("Smoothness", Range(0.0,1.0)) = 0.0
		_Specularity("Specularity", Range(1.0,20.0)) = 0.0
	}

	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma multi_compile _BUMPMAP
			#pragma multi_compile _ SHADOWS_SCREEN

			#pragma vertex vertBasic
			#pragma fragment frag
			
			#include "../includes/Common.cginc"	

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _BumpTex;
			uniform half4 _BumpTex_ST;
			uniform float _Smoothness;
			uniform float _Specularity;
			uniform half4 _Color;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float3 normal, light, view, halff;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);

				#if SHADOWS_FWD_BASE
				attenuation *= SHADOW_ATTENUATION(i);
				#endif	

				half4 tex  = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 texN = tex2D(_BumpTex, i.texcoords.xy * _BumpTex_ST.xy + _BumpTex_ST.zw);

				normal = BUMP_NORMAL(i, texN, 2 * (1 - _Smoothness));

				half3 diffuse  = _Color.rgb * tex.rgb * clamp(dot(normal,light), 0.15, 0.85);
				half3 specular = _LightColor0.rgb * tex.rgb * saturate(pow(saturate(dot(normal,halff)), _Smoothness * _Specularity*_Specularity));

				half3 final = attenuation * (diffuse + _Smoothness * specular);

				return half4(final, 1);
			}
			
			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ForwardAdd" }

			Blend One One

			CGPROGRAM

			#pragma multi_compile _BUMPMAP
			#pragma multi_compile_fwdadd_fullshadows

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../includes/Common.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _BumpTex;
			uniform half4 _BumpTex_ST;
			uniform float _Smoothness;
			uniform float _Specularity;
			uniform half4 _Color;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float3 normal, light, view, halff;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);

				#if SHADOWS_FWD_ADD
				attenuation *= SHADOW_ATTENUATION(i);
				#endif

				half4 tex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 texN = tex2D(_BumpTex, i.texcoords.xy * _BumpTex_ST.xy + _BumpTex_ST.zw);

				i.normalWorld = BUMP_NORMAL(i, texN, 2 * (1 - _Smoothness));

				half3 diffuse = _Color.rgb * tex.rgb * clamp(dot(i.normalWorld, light), 0.15, 0.85);
				half3 specular = _LightColor0.rgb * tex.rgb * saturate(pow(saturate(dot(i.normalWorld,halff)), _Smoothness * _Specularity*_Specularity));

				half3 final = attenuation * (diffuse + _Smoothness * specular);

				return half4(final, 1);
			}

			ENDCG
		}

		UsePass "CommonPass/SHADOWCASTER"
	}	
}