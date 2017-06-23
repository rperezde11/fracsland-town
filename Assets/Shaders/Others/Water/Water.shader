Shader "Custom/Nature/Water" {

	Properties{
		[KeywordEnum(Simple, Complex)] Water("Water Type", Float) = 0

		[HDR] _Color("Color", Color) = (1,1,1,1)
		_Toonify("Toon Value", Range(0.01, 0.2)) = 0.05
		
		_BumpTex("Distortion Texture", 2D) = "bump"{}
		[NoScaleOffset] _FresnelAirWaterTex("Fresnel Ramp Texture", 2D) = "white"{}
		
		_WaveIntensity("Wave Intensity", Range(0.05,1)) = 0.05
		_WaveScale("Wave Scale", Range(1,10)) = 1

		[Toggle(WATER_DEPTH)] _DepthFog("Depth Fog?", Range(0,1)) = 0
		_WaterFogIntensity("Water Fog Intensity", Range(0,1)) = 0
		
		_Speed("Water Speed", Range(0.01,0.25)) = 0.1
		_TideHeight("Tide Height", Range(0,20)) = 0.1
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent-2" }

		// This pass grabs the screen behind
		// accessible on next pass as _GrabPass
		GrabPass{

		}

		CGINCLUDE

		#include "../../includes/Common.cginc"
		#include "../../includes/UtilsBump.cginc"
		#include "../../includes/UtilsToon.cginc"

		uniform half4 _Color;
		uniform half4 _HighlightColor;
		uniform float _Toonify;
		uniform sampler2D _RefractionTex;
		uniform sampler2D _GrabTexture;
		uniform sampler2D _FresnelAirWaterTex;
		uniform sampler2D _CameraDepthTexture;
		uniform float _WaterFogIntensity;
		uniform float _Speed;
		uniform sampler2D _BumpTex;
		uniform float _WaveIntensity;
		uniform float _WaveScale;
		uniform float _TideHeight;

		#pragma multi_compile WATER_SIMPLE WATER_COMPLEX
		#pragma multi_compile _ WATER_DEPTH

		half4 ComputeWaterReflection(float3 view, float3 normalWorld, float3 posWorld)
		{
			half3 reflection = ComputeReflexDirection(view, normalWorld, posWorld);
			reflection = SampleLightProbeCubemap(reflection, 0.025);

			reflection.r = GetStairedValue(reflection.r, _Toonify);
			reflection.g = GetStairedValue(reflection.g, _Toonify);
			reflection.b = GetStairedValue(reflection.b, _Toonify);

			return half4(reflection, 1);
		}

		half4 ComputeWaterRefraction(half4 color, sampler2D refractionTex, float2 screenProjCoords, float2 screenDepthProjCoords)
		{
			half4 refraction = tex2D(refractionTex, screenProjCoords);

			#if WATER_DEPTH
				// Set 0 at observers position and 1 at far plane
				float depth = Linear01Depth(tex2D(_CameraDepthTexture, screenDepthProjCoords).r);
				// The camera used on edit mode has different far plane distance
				// this factor helps a little avoiding this issue.
				float factor = 1 + ((_ProjectionParams.x < 0) ? 3000 : 30);

				refraction = lerp(refraction, color, 0.5 + 0.5*saturate(depth*_WaterFogIntensity*factor));
			#else
				refraction = lerp(color, refraction, 0.5);
			#endif

			return refraction * _LightColor0.rgba;
		}

		ENDCG

		Pass
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fog

			struct appdata
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
				float4 tangent  : TANGENT;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos			  : SV_POSITION;
				float4 posWorld		  : TEXCOORD0;
				float3 normalWorld	  : TEXCOORD1;
				float2 uv			  : TEXCOORD2;
				float4 uv_GrabScreen  : TEXCOORD3;
				float4 uv_GrabDepth   : TEXCOORD4;

				BUMP_COORDS(5,6)
			};

			v2f vert(appdata v)
			{
				v2f o;

				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalWorld = normalize(mul(v.normal, unity_WorldToObject));
				o.posWorld.xyz += _TideHeight * _SinTime.w * o.normalWorld.xyz;
				
				o.pos = mul(UNITY_MATRIX_VP, o.posWorld);

				o.uv = v.texcoord;

				o.uv_GrabScreen = o.pos;
				o.uv_GrabDepth = o.pos;

				o.uv_GrabDepth.y *= -_ProjectionParams.x;

				o.uv_GrabScreen = ComputeGrabScreenPos(o.uv_GrabScreen);
				o.uv_GrabDepth = ComputeGrabScreenPos(o.uv_GrabDepth);

				TRANSFER_BUMP(o);
				
				o.posWorld.w = GetFogFactor(o.pos);

				return o;
			}


			#if WATER_SIMPLE

			half4 frag(v2f i) : COLOR
			{
				float attenuation;
				float ndotv;
				float3 light, view, halff;
				half4 reflection, refraction;
				half4 finalColor;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);

				float4 texN = tex2D(_BumpTex, (i.uv + _Time.x * _Speed) * _WaveScale);
				texN += tex2D(_BumpTex, i.uv - _Time.x * _Speed);
				texN *= 0.5;

				i.normalWorld = BUMP_NORMAL(i, texN, _WaveIntensity);

				i.uv_GrabScreen.xy /= i.uv_GrabScreen.w;
				i.uv_GrabDepth.xy /= i.uv_GrabDepth.w;

				refraction = ComputeWaterRefraction(_Color, _GrabTexture, i.uv_GrabScreen.xy + _WaveIntensity * 0.025 * i.normalWorld.xz, i.uv_GrabDepth.xy + _WaveIntensity * 0.025 * i.normalWorld.xz);

				// Compute cosine * length(view) instead of cosine, this way ndotv
				// is not completely black at the horizon.
				ndotv = dot((_WorldSpaceCameraPos.xyz - i.posWorld), i.normalWorld);
				
				AddFog(refraction, i.posWorld.w);

				return lerp(refraction, _LightColor0, 0.25) + GetStairedValue(1 - saturate(ndotv), 0.25);
			}

			#endif


			#if WATER_COMPLEX

			half4 frag(v2f i) : COLOR
			{
				float attenuation;
				float ndotv;
				float3 light, view, halff;
				half4 reflection, refraction;
				half4 reflectance;
				half4 finalColor;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);

				float4 texN = tex2D(_BumpTex, (i.uv + _Time.x * _Speed) * _WaveScale);
				texN += tex2D(_BumpTex, i.uv - _Time.x * _Speed);
				texN *= 0.5;

				i.normalWorld = BUMP_NORMAL(i, texN, _WaveIntensity);

				i.uv_GrabScreen.xy /= i.uv_GrabScreen.w;
				i.uv_GrabDepth.xy /= i.uv_GrabDepth.w;

				reflection = lerp(ComputeWaterReflection(view, i.normalWorld, i.posWorld), _Color, 0.2);
				refraction = ComputeWaterRefraction(_Color, _GrabTexture, i.uv_GrabScreen.xy + _WaveIntensity * 0.025 * i.normalWorld.xz, i.uv_GrabDepth.xy + _WaveIntensity * 0.025 * i.normalWorld.xz);

				ndotv = dot(view, i.normalWorld);
				reflectance = tex2D(_FresnelAirWaterTex, 0.01 + ndotv * 0.98).r; // Single channel texture values stored in alpha channel	

				finalColor = lerp(refraction, reflection, reflectance);
				AddFog(finalColor, i.posWorld.w);

				return finalColor;
			}

			#endif

			ENDCG
		}
	}

	CustomEditor "WaterShaderGUI"
}
