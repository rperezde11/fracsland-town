Shader "Custom/toon/ramp"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_RampTex("Ramp Texture", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma multi_compile _ SHADOWS_SCREEN

			#pragma vertex vert
			#pragma fragment frag

			#include "../includes/Common.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;			
			uniform sampler2D _RampTex;
			uniform half4 _RampTex_ST;

			uniform half4 _LightColor0;

			struct VertexIn {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed2 uv     : TEXCOORD0;
			};

			struct VertexOut {
				float4 pos         : SV_POSITION;
				fixed2 texcoords   : TEXCOORD0;
				float4 posWorld    : TEXCOORD1;
				float3 normalWorld : TEXCOORD2;

				#if defined(SHADOWS_SCREEN)
				SHADOW_COORDS(3)
				#endif
			};


			VertexOut vert(VertexIn i)
			{
				VertexOut o;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.texcoords = i.uv;
				o.posWorld = mul(unity_ObjectToWorld, i.vertex);
				o.normalWorld = normalize(mul(float4(i.normal, 0.0), unity_WorldToObject)).xyz;
				
				#if defined(SHADOWS_SCREEN)
				TRANSFER_SHADOW(o);
				#endif

				return o;
			}

			half4 frag(VertexOut i) : COLOR
			{
				float  attenuation;
				float3 l;

				if (_WorldSpaceLightPos0.w == 0.0)
				{
					l = normalize(_WorldSpaceLightPos0.xyz);
					attenuation = 1.0;
				}
				else
				{
					l = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					attenuation = 1.0 / length(l);
					l = normalize(l);
				}

				float3 n = normalize( i.normalWorld );
				float3 v = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

				float ndotl = dot(n,l);

				ndotl = saturate(0.5 * ndotl + 0.5);

				ndotl = 0.01 + ndotl * 0.98; // This is to avoid grey pixels

				half4 albedoTex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 rampTex   = tex2D(_RampTex, float2(ndotl, 1.0));
				
				#if defined(SHADOWS_SCREEN)
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

			#pragma multi_compile_fwdadd_fullshadows

			#pragma vertex vert
			#pragma fragment frag

			#include "../includes/Common.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _RampTex;
			uniform half4 _RampTex_ST;

			uniform half4 _LightColor0;

			struct VertexIn {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed2 uv     : TEXCOORD0;
			};

			struct VertexOut {
				float4 pos         : SV_POSITION;
				fixed2 texcoords   : TEXCOORD0;
				float4 posWorld    : TEXCOORD1;
				float3 normalWorld : TEXCOORD2;

				#if defined(SHADOWS_DEPTH) || defined(SHADOWS_CUBE)
				SHADOW_COORDS(3)
				#endif
			};


			VertexOut vert(VertexIn v)
			{
				VertexOut o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoords = v.uv;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalWorld = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject)).xyz;

				#if defined(SHADOWS_DEPTH) || defined(SHADOWS_CUBE)
				TRANSFER_SHADOW(o);
				#endif

				return o;
			}

			half4 frag(VertexOut i) : COLOR
			{
				float  attenuation;
				float3 l;

				if (_WorldSpaceLightPos0.w == 0.0)
				{
					l = normalize(_WorldSpaceLightPos0.xyz);
					attenuation = 1.0;
				}
				else
				{
					l = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					attenuation = 1.0 / length(l);
					l = normalize(l);
				}

				float3 n = normalize(i.normalWorld);
				float3 v = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

				float ndotl = dot(n,l);

				ndotl = saturate(0.5 * ndotl + 0.5);

				ndotl = 0.01 + ndotl * 0.98; // This is to avoid grey pixels

				half4 albedoTex = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 rampTex = tex2D(_RampTex, float2(ndotl, 1.0));

				#if defined(SHADOWS_DEPTH) || defined(SHADOWS_CUBE)
				attenuation = SHADOW_ATTENUATION(i);
				#endif

				return _LightColor0 * half4(rampTex * albedoTex) * attenuation;
			}

			ENDCG
		}
		
		UsePass "CommonPass/SHADOWCASTER"
	}
}