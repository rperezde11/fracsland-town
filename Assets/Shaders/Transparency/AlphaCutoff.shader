Shader "Custom/AlphaCutoff" {

	Properties{
		[NoScaleOffset] _MainTex("Main Texture", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0, 1)) = 0.2

		[HideInInspector] _BlendingSrc("BlendingSrc", Float) = 0
		[HideInInspector] _BlendingDst("BlendingSrc", Float) = 0
		[HideInInspector] _ZWrite("ZWrite", Float) = 1
		[HideInInspector] _AlphaToMaskOn("AlphaToMaskOn", Float) = 0
	}

	SubShader
	{
		Tags{ "Queue" = "AlphaTest" }

		AlphaToMask [_AlphaToMaskOn]
		ZWrite [_ZWrite]
		Blend [_BlendingSrc] [_BlendingDst]

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#pragma shader_feature ALPHA_TEST_HARD ALPHA_TEST_SMOOTH ALPHA_BLEND

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float _Cutoff;

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

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float3 normal, light;
				float ndotl;
				half4 tex, diffuse;

				tex = tex2D(_MainTex, i.uv.xy);

				#if ALPHA_TEST_HARD
				clip(tex.a - _Cutoff);
				#endif

				if (_WorldSpaceLightPos0.w == 0.0)
				{
					light = normalize(_WorldSpaceLightPos0.xyz);
				}
				else
				{
					light = normalize(_WorldSpaceLightPos0.xyz - i.posWorld.xyz);
				}
				
				normal = normalize(i.normalWorld);

				tex.a = tex.a - _Cutoff;

				ndotl = dot(normal, light);

				diffuse.rgb = tex.rgb * (0.7 + 0.3 * saturate(ndotl));
				diffuse.rgb += 0.5*diffuse.rgb*pow(saturate(ndotl), 20);
				diffuse.a = tex.a - _Cutoff;

				return diffuse;
			}

			ENDCG
		}

		//UsePass "CommonPass/SHADOWCASTER"

	}

	CustomEditor "SimpleTransparentShaderGUI"
}
