Shader "Custom/Fog" {

	Properties{
		_MainTex("Main Texture", 2D) = "white" {}

		[HideInInspector] _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.2
		[HideInInspector] _Speed("Speed", Range(0,1)) = 0.5
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent-1" }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Cutoff;
			uniform float _Speed;

			uniform half4 _LightColor0;

			struct appdata
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos         : SV_POSITION;
				float4 posWorld    : TEXCOORD0;
				float3 normalWorld : TEXCOORD1;
				float2 uv          : TEXCOORD2;
			};

			v2f vert(appdata v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalWorld = normalize(mul(v.normal, unity_WorldToObject));
				
				o.uv = v.texcoord;
				o.uv.y += _Speed*_Time.x;
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float3 normal, light;
				float ndotl;
				float cutoff;
				half4 tex, diffuse;

				cutoff = saturate(_Cutoff * _SinTime.x);

				tex = tex2D(_MainTex, i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				if (_WorldSpaceLightPos0.w == 0.0)
				{
					light = normalize(_WorldSpaceLightPos0.xyz);
				}
				else
				{
					light = normalize(_WorldSpaceLightPos0.xyz - i.posWorld.xyz);
				}
				
				normal = normalize(i.normalWorld);

				ndotl = dot(normal, light);

				diffuse.rgb = tex.rgb * (0.7 + 0.3 * saturate(ndotl));
				diffuse.a = tex.a - cutoff;

				return diffuse;
			}

			ENDCG
		}

		/*
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			Name "SHADOWCASTER"

			CGPROGRAM

			#pragma target 3.0 // TODO: Si lo quito en principio no pasa nada

			#pragma multi_compile_shadowcaster

			#define SEMITRANSPARENT_SHADOWS 1

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			uniform sampler2D _MainTex;
			uniform sampler3D _DitherMaskLOD;
			uniform float _Cutoff;

			struct appdata {
				float4 vertex : POSITION;
				half3 normal  : NORMAL;
				float2 texcoords : TEXCOORD0;
			};

			struct v2f {
				
				float4 pos : SV_POSITION;
				#if defined(SHADOWS_CUBE)
					half3 light : TEXCOORD0;
				#endif

				#if SEMITRANSPARENT_SHADOWS
					float2 uv : TEXCOORD1;
				#endif
			};

			struct iv2f {

				#if SEMITRANSPARENT_SHADOWS
					UNITY_VPOS_TYPE vpos : VPOS;
				#else
					float4 pos : SV_POSITION;
				#endif

				#if defined(SHADOWS_CUBE)
					half3 light : TEXCOORD0;
				#endif

				#if SEMITRANSPARENT_SHADOWS
					float2 uv : TEXCOORD1;
				#endif
			};

			v2f vert(appdata v)
			{
				v2f o;

				#if defined(SHADOWS_CUBE)
					o.pos = UnityObjectToClipPos(v.vertex.xyz);
					o.light = mul(unity_ObjectToWorld, v.vertex).xyz - _LightPositionRange.xyz;
				#else
					o.pos = UnityClipSpaceShadowCasterPos(v.vertex.xyz, v.normal);
					o.pos = UnityApplyLinearShadowBias(o.pos);

					#if SEMITRANSPARENT_SHADOWS
					o.uv = v.texcoords;
					o.uv.y += _Time.x;
					#endif
				#endif

				return o;
			}

			half4 frag(iv2f i) : SV_TARGET
			{
				#if defined(SHADOWS_CUBE)
					float depth = length(i.light) + unity_LightShadowBias.x;
					depth *= _LightPositionRange.w;
					return UnityEncodeCubeShadowDepth(depth);
				#else
					float alpha = tex2D(_MainTex, i.uv).a;
					float dither = tex3D(_DitherMaskLOD, float3(i.vpos.xy*0.25, alpha*0.9375)).a;
					clip(dither - 0.01);
					return 0;
				#endif
			}

			ENDCG
		}
		*/

	}
}
