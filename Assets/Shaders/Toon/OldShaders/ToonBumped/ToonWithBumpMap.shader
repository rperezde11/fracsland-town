Shader "Custom/toon/bumped"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_BumpTex("Normal Texture", 2D) = "bump" {}
		_RampTex("Ramp Texture", 2D) = "white" {}
		_Bumpiness("Bumpiness", Range(0.0, 20.0)) = 9.0
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

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
			uniform sampler2D _RampTex;
			uniform half4 _RampTex_ST;
			uniform float _Bumpiness;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float3 normal, light;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, attenuation);

				float4 texN = tex2D(_BumpTex, i.texcoords.xy * _BumpTex_ST.xy + _BumpTex_ST.zw);

				normal = BUMP_NORMAL(i, texN, _Bumpiness);

				float ndotl = dot(normal,light);

				ndotl = 0.5 + 0.5 * ndotl;
				ndotl = 0.01 + ndotl * 0.98; // This is to avoid grey pixels

				half4 albedoTex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 rampTex   = tex2D(_RampTex, float2(ndotl, 1.0));

				#if SHADOWS_FWD_BASE
				attenuation *= SHADOW_ATTENUATION(i);
				#endif

				return _LightColor0 * half4(rampTex * albedoTex) * attenuation;
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
			uniform sampler2D _RampTex;
			uniform half4 _RampTex_ST;
			uniform float _Bumpiness;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float3 normal, light;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, attenuation);

				float4 texN = tex2D(_BumpTex, i.texcoords.xy * _BumpTex_ST.xy + _BumpTex_ST.zw);

				normal = BUMP_NORMAL(i, texN, _Bumpiness);

				float ndotl = dot(normal, light);

				ndotl = 0.5 + 0.5 * ndotl;
				ndotl = 0.01 + ndotl * 0.98; // This is to avoid grey pixels

				half4 albedoTex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 rampTex = tex2D(_RampTex, float2(ndotl, 1.0));

				#if SHADOWS_FWD_ADD
				attenuation = SHADOW_ATTENUATION(i);
				#endif

				return _LightColor0 * half4(rampTex * albedoTex) * attenuation;
			}

			ENDCG
		}

		UsePass "CommonPass/SHADOWCASTER"
	}
}