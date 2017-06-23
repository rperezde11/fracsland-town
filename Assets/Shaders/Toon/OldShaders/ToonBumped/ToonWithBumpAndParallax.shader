Shader "Custom/bumped/parallax"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_BumpTex("Normal Map Texture", 2D) = "bump" {}
		_ParallaxTex("Parallax Map Texture", 2D) = "white" {}
		[HideInInspector] _ParallaxFactor("Parallax height factor", Float) = 0.025
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
		_ParallaxMaxDistance("Parallax distance limit", Range(0.1, 100.0)) = 3
		[KeywordEnum(None, Bare, Steep, Occlussion)] _ParallaxType ("Parallax Type", Float) = 1
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma multi_compile _BUMPMAP
			#pragma multi_compile _PARALLAX_MAP
			#pragma multi_compile _ SHADOWS_SCREEN

			#pragma shader_feature _PARALLAXTYPE_NONE _PARALLAXTYPE_BARE _PARALLAXTYPE_STEEP _PARALLAXTYPE_OCCLUSSION

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../includes/Common.cginc"
			#include "../includes/UtilsBump.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _BumpTex;
			uniform half4 _BumpTex_ST;
			uniform sampler2D _ParallaxTex;
			uniform half4 _ParallaxTex_ST;
			uniform float _ParallaxFactor;
			uniform float _ParallaxMaxDistance;
			uniform float _Smoothness;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float3 normal, light;
				half4 diffuse, final;
				float bumpiness;
				float ndotl;
				float attenuation;

				bumpiness = 1 - _Smoothness;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, attenuation);

				CalculateParallaxTexcoords(i.viewSurface, bumpiness * _ParallaxFactor, _ParallaxMaxDistance, i.texcoords, _ParallaxTex, _ParallaxTex_ST);
				normal = CalculateBumpNormals(2 * bumpiness, i, _BumpTex, _BumpTex_ST);

				ndotl = 0.5 + 0.5 * dot(normal,light);

				#if SHADOWS_FWD_BASE
				attenuation *= SHADOW_ATTENUATION(i);
				#endif

				diffuse = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw) * ndotl;

				final = attenuation * _LightColor0 * diffuse;

				return final;
			}
			
			ENDCG
		}
		
		Pass
		{
			Tags{ "LightMode" = "ForwardAdd" }

			Blend One One

			CGPROGRAM

			#pragma multi_compile _BUMPMAP
			#pragma multi_compile _PARALLAX_MAP
			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile UNITY_PASS_FORWARDADD

			#pragma shader_feature _PARALLAXTYPE_NONE _PARALLAXTYPE_BARE _PARALLAXTYPE_STEEP _PARALLAXTYPE_OCCLUSSION

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../includes/Common.cginc"
			#include "../includes/UtilsBump.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _BumpTex;
			uniform half4 _BumpTex_ST;
			uniform sampler2D _ParallaxTex;
			uniform half4 _ParallaxTex_ST;
			uniform float _ParallaxFactor;
			uniform float _ParallaxMaxDistance;
			uniform float _Smoothness;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float ndotl;
				float bumpiness;
				float3 normal, light;
				half4 diffuse, final;

				bumpiness = 1 - _Smoothness;

				InitFragmentVectors(i.posWorld, i.normalWorld, normal, light, attenuation);

				CalculateParallaxTexcoords(i.viewSurface, bumpiness * _ParallaxFactor, _ParallaxMaxDistance, i.texcoords, _ParallaxTex, _ParallaxTex_ST);
				normal = CalculateBumpNormals(2 * bumpiness, i, _BumpTex, _BumpTex_ST);

				ndotl = 0.5 * 0.5 + dot(normal,light);

				diffuse = tex2D(_MainTex, i.texcoords.xy * _MainTex_ST.xy + _MainTex_ST.zw) * ndotl;

				#if SHADOWS_FWD_BASE
				attenuation *= SHADOW_ATTENUATION(i);
				#endif

				final = _LightColor0 * attenuation * diffuse;

				return final;
			}

			ENDCG
		}
		
		UsePass "CommonPass/SHADOWCASTER"
	}
}