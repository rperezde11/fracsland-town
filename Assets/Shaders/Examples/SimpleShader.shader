Shader "Custom/Examples/SimpleShader"
{
	Properties
	{
		_MainTex("Albedo Texture", 2D) = "white" {}
		_Color("Albedo Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Range(0,1)) = 0.5
	}

	SubShader
	{

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vertBasic
			#pragma fragment frag

			#include "../includes/Common.cginc"

			uniform half4 _LightColor0;

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform half4 _Color;
			uniform float _Shininess;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float ndotl, ndoth;
				float3 normal, light, view, halff;
				float4 diffuse, specular, final;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);
				attenuation *= SHADOW_ATTENUATION(i);

				ndotl = saturate(dot(light, i.normalWorld));
				ndoth = saturate(pow(dot(halff, i.normalWorld), _Shininess * 128));

				diffuse  = ndotl * _Color * tex2D(_MainTex, i.texcoords.xy + _MainTex_ST.xy + _MainTex_ST.zw);
				specular = ndotl * ndoth * _LightColor0;

				final = (diffuse + specular) * attenuation;
				
				return final;
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

			#include "../includes/Common.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform half4 _Color;
			uniform float _Shininess;

			uniform half4 _LightColor0;

			half4 frag(v2f_basic i) : COLOR
			{
				float attenuation;
				float ndotl, ndoth;
				float3 normal, light, view, halff;
				float4 diffuse, specular, final;

				InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);
				attenuation *= SHADOW_ATTENUATION(i);

				ndotl = saturate(dot(light, i.normalWorld));
				ndoth = saturate(pow(dot(halff, i.normalWorld), _Shininess * 128));

				diffuse = ndotl * _Color * tex2D(_MainTex, i.texcoords.xy + _MainTex_ST.xy + _MainTex_ST.zw);
				specular = ndotl * ndoth * _LightColor0;

				final = (diffuse + specular) * attenuation;

				return final;
			}

			ENDCG
		}
		
	}
}