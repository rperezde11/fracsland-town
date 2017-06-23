Shader "Custom/toon/ramp-emit"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_EmitTex("Emission (A)", 2D) = "black" {}
		_EmitColor("Emission Color", Color) = (1,1,1,1)
		_Blending("Emission Blending", Range(0.0, 1.0)) = 0.7
		_RampTex("Ramp Texture", 2D) = "white" {}

		[Toggle(FLICKER_ENABLED)] _Flicker("Flicker Lights", Float) = 1
		_FlickerSpeed("_Flicker Speed", Range(1.0, 15.0)) = 10.0
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile _ VERTEXLIGHT_ON
			#pragma multi_compile _ FLICKER_ENABLED

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../includes/Common.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;			
			uniform sampler2D _EmitTex;
			uniform half4 _EmitTex_ST;		
			uniform sampler2D _RampTex;
			uniform half4 _RampTex_ST;
			uniform half4 _EmitColor;
			uniform half _Blending;
			uniform half _FlickerSpeed;

			uniform half4 _LightColor0;
			 
			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float3 normal, light;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, attenuation);

				float ndotl = dot(normal, light);

				ndotl = saturate(0.5 + 0.5 * ndotl);

				ndotl = 0.01 + ndotl * 0.98; // This is to avoid grey pixels

				half4 albedoTex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 emitTex   = tex2D(_EmitTex, i.texcoords.xy * _EmitTex_ST.xy + _EmitTex_ST.zw);
				half4 rampTex   = tex2D(_RampTex, float2(ndotl, 1.0));

				#if SHADOWS_FWD_BASE
				attenuation *= SHADOW_ATTENUATION(i);
				#endif

				float4 finalLight = _LightColor0 * half4(rampTex * albedoTex) * attenuation;

				if (emitTex.a > 0.1)
				{
					float flicker = 1.0;

					#if FLICKER_ENABLED
					flicker = abs((_SinTime.w + _CosTime.w) * sin(_FlickerSpeed * _Time.y));
					#endif

					finalLight += flicker * ((1.0 - _Blending) * finalLight + _Blending * (emitTex.a * _EmitColor));
				}

				return finalLight;
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

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _RampTex;
			uniform half4 _RampTex_ST;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float3 normal, light;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, attenuation);

				float ndotl = dot(normal, light);

				ndotl = saturate(0.5 + 0.5 * ndotl);

				ndotl = 0.01 + ndotl * 0.98; // This is to avoid grey pixels

				half4 albedoTex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 rampTex = tex2D(_RampTex, float2(ndotl, 1.0));

				#if SHADOWS_FWD_ADD
				attenuation = SHADOW_ATTENUATION(i);
				#endif

				return  _LightColor0 * half4(rampTex * albedoTex) * attenuation;
			}

			ENDCG
		}
		

		UsePass "CommonPass/SHADOWCASTER"
	}
}