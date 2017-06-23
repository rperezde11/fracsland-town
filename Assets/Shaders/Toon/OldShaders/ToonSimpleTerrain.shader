Shader "Custom/toon/simple-terrain" {

	Properties {
		_Factor ("Light Factor", Range(1.0, 5.0)) = 2.0
		_Bumpiness("Bumpiness", Range(0.0, 5.0)) = 1.0
		_Tiling("Tiling", Range(0.0, 5.0)) = 1.0

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

			#pragma multi_compile_fog

			#pragma vertex vertTerrain
			#pragma fragment frag

			#include "../includes/TerrainCommon.cginc"
			
			uniform half4 _LightColor0;

			half4 frag (v2f_terrain i) : COLOR
			{
				half4 color;
				half4 splatControl;
				half4 final;
				float3 light, normal;
				float attenuation;
				float ndotl;
				float colorIntensity;

				SplatMapUV IN;

				IN.uv_Splat0 = i.pack_0.xy;
				IN.uv_Splat1 = i.pack_0.zw;
				IN.uv_Splat2 = i.pack_1.xy;
				IN.uv_Splat3 = i.pack_1.zw;
				IN.tc_Control = i.pack_c.xy;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, attenuation);

				splatControl = tex2D(_Control, IN.tc_Control);
				
				color  = splatControl.r * tex2D(_Splat0, IN.uv_Splat0);
				color += splatControl.g * tex2D(_Splat1, IN.uv_Splat1);
				color += splatControl.b * tex2D(_Splat2, IN.uv_Splat2);
				color += splatControl.a * tex2D(_Splat3, IN.uv_Splat3);
				
				#if _TERRAIN_NORMAL_MAP
				half4 texN;
				texN  = splatControl.r * tex2D(_Normal0, _Tiling * IN.uv_Splat0);
				texN += splatControl.g * tex2D(_Normal1, _Tiling * IN.uv_Splat1);
				texN += splatControl.b * tex2D(_Normal2, _Tiling * IN.uv_Splat2);
				texN += splatControl.a * tex2D(_Normal3, _Tiling * IN.uv_Splat3);

				BumpVectors b;
				b._TangentWorld  = i.tangentWorld;
				b._BinormalWorld = i.binormalWorld;
				b.normalWorld    = i.normalWorld;

				normal = BUMP_NORMAL(b, texN, _Bumpiness);
				#endif

				ndotl = attenuation * saturate(dot(normal, light));

				#if SHADOWS_FWD_BASE
				attenuation = SHADOW_ATTENUATION(i);
				#endif

				final = ndotl * _LightColor0 * color * attenuation;

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
			#pragma multi_compile _ _TERRAIN_NORMAL_MAP

			#pragma vertex vertTerrain
			#pragma fragment frag

			#include "../includes/UtilsToon.cginc"
			#include "../includes/TerrainCommon.cginc"

			uniform float _Factor;

			half4 frag(v2f_terrain i) : COLOR
			{
				half4 color;
				half4 splatControl;
				float3 light, normal;
				float attenuation;
				float ndotl;
				float colorIntensity;

				SplatMapUV IN;

				IN.uv_Splat0 = i.pack_0.xy;
				IN.uv_Splat1 = i.pack_0.zw;
				IN.uv_Splat2 = i.pack_1.xy;
				IN.uv_Splat3 = i.pack_1.zw;
				IN.tc_Control = i.pack_c.xy;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, attenuation);

				normal = i.normalWorld;

				splatControl = tex2D(_Control, IN.tc_Control);

				color = splatControl.r * tex2D(_Splat0, IN.uv_Splat0);
				color += splatControl.g * tex2D(_Splat1, IN.uv_Splat1);
				color += splatControl.b * tex2D(_Splat2, IN.uv_Splat2);
				color += splatControl.a * tex2D(_Splat3, IN.uv_Splat3);

				#if _TERRAIN_NORMAL_MAP
				half4 texN;
				texN = splatControl.r * tex2D(_Normal0, _Tiling * IN.uv_Splat0);
				texN += splatControl.g * tex2D(_Normal1, _Tiling * IN.uv_Splat1);
				texN += splatControl.b * tex2D(_Normal2, _Tiling * IN.uv_Splat2);
				texN += splatControl.a * tex2D(_Normal3, _Tiling * IN.uv_Splat3);

				BumpVectors b;
				b._TangentWorld = i.tangentWorld;
				b._BinormalWorld = i.binormalWorld;
				b.normalWorld = i.normalWorld;

				normal = BUMP_NORMAL(b, texN, _Bumpiness);
				#endif

				ndotl = attenuation * saturate(dot(normal, light));

				attenuation = GetTerrainToonAttenuation(i.posWorld, light, normal, _Factor);

				#if SHADOWS_FWD_ADD
				attenuation *= SHADOW_ATTENUATION(i);
				#endif

				float3 finalLight = attenuation * _LightColor0.rgb * color.rgb * GetTerrainToonIntensity(attenuation);
				return half4(finalLight, 1);
			}

			ENDCG
		}

		UsePass "CommonPass/SHADOWCASTER"
	}

}