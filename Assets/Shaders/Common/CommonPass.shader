Shader "CommonPass"
{
	SubShader 
	{	
		
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			Name "SHADOWCASTER"

			CGPROGRAM

			#pragma target 3.0 // TODO: Si lo quito en principio no pasa nada

			#pragma multi_compile_shadowcaster
		
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			uniform sampler2D _MainTex;
			uniform sampler3D _DitherMaskLOD;
			uniform float _AlphaCutoff;

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

				#if SEMITRANSPARENT_SHADOWS
					o.uv = v.texcoords;
				#endif

				#if defined(SHADOWS_CUBE)
					o.pos = UnityObjectToClipPos(v.vertex.xyz);
					o.light = mul(unity_ObjectToWorld, v.vertex).xyz - _LightPositionRange.xyz;
				#else
					o.pos = UnityClipSpaceShadowCasterPos(v.vertex.xyz, v.normal);
					o.pos = UnityApplyLinearShadowBias(o.pos);
				#endif

				return o;
			}

			half4 frag(iv2f i) : SV_TARGET
			{
				#if SEMITRANSPARENT_SHADOWS
					float alpha = tex2D(_MainTex, i.uv).a * (1 - _AlphaCutoff);
					float dither = tex3D(_DitherMaskLOD, float3(i.vpos.xy*0.25, 0.5*0.9375)).a;
					clip(dither - 0.01);
				#endif

				#if defined(SHADOWS_CUBE)
					float depth = length(i.light) + unity_LightShadowBias.x;
					depth *= _LightPositionRange.w;
					return UnityEncodeCubeShadowDepth(depth);
				#endif

				return 0;
			}

			ENDCG
		}


		Pass
		{
			Cull Front

			NAME "OUTLINE"

			CGPROGRAM

			#pragma multi_compile _ _CONTOURTYPE_OUTLINE

			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float _OutlineWidth;
			uniform half4 _OutlineColor;

			struct appdata
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
			};

			struct v2g
			{
				float4 posWorld    : TEXCOORD1;
				float3 normalWorld : TEXCOORD2;
			};


			struct g2f
			{
				float4 pos : SV_POSITION;
			};

			v2g vert(appdata v)
			{
				v2g o;

				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalWorld = normalize(mul(v.normal, unity_WorldToObject).xyz);

				return o;
			}

			[maxvertexcount(3)]
			void geo(triangle v2g info[3], inout TriangleStream<g2f> triStream)
			{
				g2f p1, p2, p3;

				#if !_CONTOURTYPE_OUTLINE
				_OutlineWidth = 0;
				#endif
				
				p1.pos = mul(UNITY_MATRIX_VP, info[0].posWorld + _OutlineWidth * float4(info[0].normalWorld, 0));
				p2.pos = mul(UNITY_MATRIX_VP, info[1].posWorld + _OutlineWidth * float4(info[1].normalWorld, 0));
				p3.pos = mul(UNITY_MATRIX_VP, info[2].posWorld + _OutlineWidth * float4(info[2].normalWorld, 0));

				triStream.Append(p1);
				triStream.Append(p2);
				triStream.Append(p3);

				triStream.RestartStrip();
			}

			half4 frag(g2f i) : COLOR
			{
				return _OutlineColor;
			}

			ENDCG
		}

	}
}