Shader "Custom/Toon/Terrain" {

	Properties {
		_Factor ("Light Factor", Range(1.0, 5.0)) = 2.0
		_Bumpiness("Bumpiness", Range(0.0, 1.0)) = 1.0
		_Tiling("Tiling", Range(0.0, 5.0)) = 1.0
		[KeywordEnum(None, Bare, Steep, Occlussion)] _ParallaxType("Parallax Type", Float) = 1
		[HideInInspector] _ParallaxFactor("Parallax height factor", Float) = 0.01

		[NoScaleOffset] _Displacement0("Displacement 0", 2D) = "black" {}
		[NoScaleOffset] _Displacement1("Displacement 1", 2D) = "black" {}
		[NoScaleOffset] _Displacement2("Displacement 2", 2D) = "black" {}
		[NoScaleOffset] _Displacement3("Displacement 3", 2D) = "black" {}

		[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0("Normal 0 (R)", 2D) = "bump" {}
		// used in fallback on old cards & base map
		[HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
	
		Pass {
			
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile _TERRAIN_NORMAL_MAP
			#pragma multi_compile _TERRAIN_PARALLAX_MAP

			#pragma multi_compile_fog

			#pragma shader_feature _PARALLAXTYPE_NONE _PARALLAXTYPE_BARE _PARALLAXTYPE_STEEP _PARALLAXTYPE_OCCLUSSION

			#pragma vertex vertTerrain
			#pragma fragment frag

			#include "../includes/TerrainCommon.cginc"
			#include "../includes/TerrainBump.cginc"
			#include "../includes/UtilsToon.cginc"

			half4 frag (v2f_terrain i) : COLOR
			{
				float attenuation;
				float3 light, normal;
				half3 diffuse;
				half4 color;
				half4 splatControl;
				half4 final;

				SplatMapUV IN;

				IN.uv_Splat0 = i.pack_0.xy;
				IN.uv_Splat1 = i.pack_0.zw;
				IN.uv_Splat2 = i.pack_1.xy;
				IN.uv_Splat3 = i.pack_1.zw;
				IN.tc_Control = i.pack_c.xy;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, attenuation);

				splatControl = tex2D(_Control, IN.tc_Control);
				
				#if _TERRAIN_PARALLAX_MAP
				CalculateParallaxTexcoords(i.viewSurface, _Bumpiness * _ParallaxFactor, splatControl, IN, _Tiling, 0);
				#endif

				normal = CalculateBumpNormals(splatControl, _Bumpiness, i, IN, _Tiling, 0);

				SET_SPLAT_TEXTURES(TEX_S, _Splat);
				SplatMapVal(splatControl, TEX_S, IN, _Tiling, 0, color);

				#if SHADOWS_FWD_BASE
				attenuation *= SHADOW_ATTENUATION(i);
				#endif

				diffuse.x = GetSoftStairedValue(saturate(dot(normal, light)), 0.2, 0.05);

				diffuse = diffuse.x * color.rgb;

				final = attenuation * _LightColor0.rgba * float4(diffuse, 1);

				AddFog(final, i.posWorld.w);

				return final;
			}

			ENDCG
		}

		
		Pass {

			Tags{ "LightMode" = "ForwardAdd" }

			Blend One One

			CGPROGRAM

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile _TERRAIN_NORMAL_MAP
			#pragma multi_compile _TERRAIN_PARALLAX_MAP

			#pragma vertex vertTerrain
			#pragma fragment frag

			#include "../includes/UtilsToon.cginc"
			#include "../includes/TerrainCommon.cginc"
			#include "../includes/TerrainBump.cginc"

			uniform half _Factor;

			half4 frag(v2f_terrain i) : COLOR
			{
				half4 color;
				half4 splatControl;
				float3 light, normal;
				float3 final;
				float attenuation;

				SplatMapUV IN;

				IN.uv_Splat0 = i.pack_0.xy;
				IN.uv_Splat1 = i.pack_0.zw;
				IN.uv_Splat2 = i.pack_1.xy;
				IN.uv_Splat3 = i.pack_1.zw;
				IN.tc_Control = i.pack_c.xy;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, attenuation);

				splatControl = tex2D(_Control, IN.tc_Control);

				normal = CalculateBumpNormals(splatControl, _Bumpiness * 3, i, IN, _Tiling, 0);

				SET_SPLAT_TEXTURES(TEX_S, _Splat);
				SplatMapVal(splatControl, TEX_S, IN, _Tiling, 0, color);

				attenuation = GetTerrainToonAttenuation(i.posWorld, light, normal, _Factor);

				#if SHADOWS_FWD_ADD
				attenuation *= SHADOW_ATTENUATION(i);
				#endif

				final = attenuation * _LightColor0.rgb * color.rgb * GetTerrainToonIntensity(attenuation);

				return half4(final, 1);
			}

			ENDCG
		}
		
		UsePass "CommonPass/SHADOWCASTER"
	}

}