Shader "Custom/Grass-CrossBillboard" {

	Properties{
		[NoScaleOffset] _MainTex("Main Texture", 2D) = "white" {}
		_TintColor("Tint Albedo", Color) = (1,1,1,1)
		_Cutoff("Alpha Cutoff", Range(0, 1)) = 0.2
		_Scale("Size", Range(0, 1)) = 0
		_Angle("Angle", Range(-90, 180)) = 0
		
		[HideInInspector] _WindDirection("Wind Direction", Vector) = (0.1,0,0.5,0.1)
		[HideInInspector] _WindIntensity("Wind Intensity", Range(0, 1)) = 0.5
		[HideInInspector] _Seed("-", Float) = 1.0
	}

	SubShader
	{
		Tags{ 
			"Queue"           = "AlphaTest" 
			"IgnoreProjector" = "True"
			"DisableBatching" = "True"
			"RenderType"      = "Transparent"
		}

		Cull Back
		AlphaToMask On
		
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fog

			#pragma shader_feature ALWAYS_FOLLOW_Y HALF_FOLLOW_Y NEVER_FOLLOW_Y

			#include "UnityCG.cginc"
			#include "../../includes/FogCommon.cginc"

			uniform sampler2D _MainTex;
			uniform half4 _TintColor;
			uniform float _Cutoff;
			uniform vector _WindDirection;
			uniform float _WindIntensity;
			uniform float _Seed;
			uniform float _SinSeed;
			uniform float _Angle;
			uniform float _Scale;

			uniform half4 _LightColor0;

			float AbsSin(float x)
			{
				float b;
				float pi = 3.14159;
				fixed n_pi_quart = -0.7853981;

				float positive_parabole = (floor(x / pi) % 2) == 1 ? -1 : 1;

				x = positive_parabole * (abs(x) % pi);
				b = positive_parabole * x / pi;

				return n_pi_quart * x * (b - 1);
			}

			float3 CalculateGrassDirection(float4 worldPos, float height, float3 windDirection, float windIntensity)
			{
				windDirection = normalize(windDirection);

				float phi = _SinSeed;
				float omega = 1;
				float amp = windIntensity + _Seed;

				float movement = 0.5 * amp * sin(omega*_Time.w + phi);

				return height * height * windIntensity * windDirection * (3 + movement);
			}

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

				float4 posCentreWorld = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - posCentreWorld.xyz);

				#if ALWAYS_FOLLOW_Y
				float3 upDir = UNITY_MATRIX_V[1].xyz;
				#elif HALF_FOLLOW_Y
				float3 upDir = normalize(UNITY_MATRIX_V[1].xyz + float3(0,1,0));
				#else
				float3 upDir = float3(0, 1, 0);
				#endif

				float3 rightDir = normalize(cross(viewDir, normalize(upDir)));

				viewDir = -normalize(cross(rightDir, upDir));
				
				// from angle to radians
				// 0.01745329 = pi / 180 
				float sine = sin(_Angle*0.017453);
				float cosine = sqrt(1 - sine*sine);
				
				rightDir = float3(rightDir.x * cosine - rightDir.z * sine, rightDir.y, rightDir.x * sine + rightDir.z * cosine);

				o.posWorld = posCentreWorld - (float4((0.25 + _Scale) * upDir * v.vertex.z, 0) + float4((0.25 + _Scale) * rightDir * v.vertex.x, 0));

				float3 dir = CalculateGrassDirection(o.posWorld, v.texcoord.y, _WindDirection, _WindIntensity);
				o.posWorld += float4(dir, 0);

				o.pos = mul(UNITY_MATRIX_VP, o.posWorld);
				o.normalWorld = viewDir;
				o.uv = v.texcoord;

				o.posWorld.w = GetFogFactor(o.pos);

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float3 normal, light;
				float ndotl;
				half4 tex, diffuse;

				tex = tex2D(_MainTex, i.uv.xy);

				clip(tex.a - _Cutoff);

				if (_WorldSpaceLightPos0.w == 0.0)
				{
					light = normalize(_WorldSpaceLightPos0.xyz);
				}
				else
				{
					light = normalize(_WorldSpaceLightPos0.xyz - i.posWorld.xyz);
				}
				
				normal = normalize(i.normalWorld);

				tex.a = i.uv.y > 0.95 ? 0 : tex.a;

				ndotl = dot(normal, light);

				diffuse = _LightColor0 * _TintColor * tex * (0.7 + 0.3 * saturate(ndotl));

				AddFog(diffuse, i.posWorld.w);

				diffuse.a = tex.a;

				return diffuse;
			}

			ENDCG
		}

	}

}
