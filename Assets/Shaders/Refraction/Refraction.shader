Shader "Custom/Transparent/Refraction" {

	Properties{
		[HDR] _Color("Color", Color) = (1,1,1,1)
		_N1("N1", Range(0.75,3)) = 1
		_N2("N2", Range(0.75,3)) = 1
		
		_BumpTex("Distortion Texture", 2D) = "bump"{}
		_Transparency("Transparency", Range(0,1)) = 0.5
		_Smoothness("Smoothness", Range(0,1)) = 0.5

		[Toggle(REFLECTION_ON)]       _ReflectionOn("Reflection On?", Float) = 1.0
		[Toggle(REFLECTION_OPTIMIZE)] _Optimize("Optimize Reflection", Float) = 1.0
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vertBasic
			#pragma fragment frag

			#pragma multi_compile _REFRACTION_PERFECT
			#pragma multi_compile _BUMPMAP

			#pragma shader_feature _ REFLECTION_OPTIMIZE
			#pragma shader_feature _ REFLECTION_ON

			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile_fog
			
			#include "../includes/Common.cginc"

			uniform half4 _Color;							
			uniform float _N1;								
			uniform float _N2;

			uniform sampler2D _BumpTex;
			uniform float4 _BumpTex_ST;
			uniform float _Smoothness;
			uniform float _Transparency;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation, refractionCoefficient, weight;
				float3 light, view, halff;
				float3 refraction, reflection;
				float4 texN;
				half4 refractionColor, reflectionColor, finalColor;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, attenuation);

				refractionCoefficient = _N1/_N2;
				weight = 0;

				texN = tex2D(_BumpTex, i.texcoords.xy * _BumpTex_ST.xy + _BumpTex_ST.zw);
				i.normalWorld = BUMP_NORMAL(i, texN, 1 - _Smoothness);

				refraction = ComputeRefractDirection(view, i.normalWorld, refractionCoefficient, i.posWorld);

				reflectionColor = SampleReflectionFragmentShader(i, view, 1 - _Smoothness);
				refractionColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, refraction, 1 - _Transparency);

				finalColor = _LightColor0 * _Color * lerp(refractionColor, reflectionColor, ComputeSchlicksReflectance(_N1, _N2, view, i.normalWorld));

				AddFog(finalColor, i.posWorld.w);
				
				return finalColor;
			}

			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ForwardAdd" }

			Blend One One
				
			CGPROGRAM

			#pragma vertex vertBasic
			#pragma fragment frag

			#pragma multi_compile _REFRACTION_PERFECT
			#pragma multi_compile _BUMPMAP

			#pragma shader_feature _ REFLECTION_OPTIMIZE
			#pragma shader_feature _ REFLECTION_ON

			#pragma multi_compile_fwdadd_fullshadows

			#include "../includes/Common.cginc"

			uniform half4 _Color;
			uniform float _N1;
			uniform float _N2;

			uniform sampler2D _BumpTex;
			uniform float4 _BumpTex_ST;
			uniform float _Smoothness;
			uniform float _Transparency;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation, refractionCoefficient, weight;
				float3 light, view, halff;
				float3 refraction, reflection;
				float4 texN;
				half4 refractionColor, reflectionColor, finalColor;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, attenuation);

				refractionCoefficient = _N1 / _N2;
				weight = 0;

				texN = tex2D(_BumpTex, i.texcoords.xy * _BumpTex_ST.xy + _BumpTex_ST.zw);
				i.normalWorld = BUMP_NORMAL(i, texN, 1 - _Smoothness);

				refraction = ComputeRefractDirection(view, i.normalWorld, refractionCoefficient, i.posWorld);

				reflectionColor = SampleReflectionFragmentShader(i, view, 1 - _Smoothness);
				refractionColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, refraction, 1 - _Transparency);

				finalColor = attenuation * _LightColor0 * _Color * lerp(refractionColor, reflectionColor, ComputeSchlicksReflectance(_N1, _N2, view, i.normalWorld));

				AddFog(finalColor, i.posWorld.w);

				return finalColor;
			}

			ENDCG
		}

	}
}
