#ifndef UTILS_SHADOW_INCLUDE
#define UTILS_SHADOW_INCLUDE

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

			struct appdata {
				float4 vertex  : POSITION;
				half3 normal : NORMAL;
			};

			#if defined(SHADOWS_CUBE)

			struct v2f {
				float4 pos : SV_POSITION;
				half3 light : TEXCOORD0;
			};	

			v2f vert(appdata i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.vertex.xyz);
				o.light = mul(unity_ObjectToWorld, i.vertex).xyz - _LightPositionRange.xyz;

				return o;
			}

			half4 frag(v2f i) : SV_TARGET
			{
				float depth = length(i.light) + unity_LightShadowBias.x;

				depth *= _LightPositionRange.w;

				return UnityEncodeCubeShadowDepth(depth);
			}

			#else // !SHADOWS CUBE

			float4 vert(appdata i) : SV_POSITION
			{
				float4 pos = UnityClipSpaceShadowCasterPos(i.vertex.xyz, i.normal);
				return UnityApplyLinearShadowBias(pos);
			}

			half4 frag() : SV_TARGET
			{
				return 0;
			}

			#endif

			ENDCG
		}

	}
}

#endif