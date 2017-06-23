Shader "Custom/Grass/GeometryGrass" {
	
	Properties {
		_Color("Grass Color", Color) = (0,1,0,1)
		_Height("Height", Range(0.0, 10.0)) = 0
		_Sections("Sections", Range(1,4)) = 2
		_WindIntensity("Wind Intensity", Range(0.0, 1.0)) = 0.5
		_WindDirection("Wind Direction", Vector) = (0,0,0,0)
	}

	SubShader 
	{	
		Tags{ "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			uniform half4 _Color;
			uniform float _Height;
			uniform float _Sections;
			uniform float _WindIntensity;
			uniform vector _WindDirection;

			uniform half4 _LightColor0;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2g
			{
				float4 vertex : TEXCOORD0;
			};

			struct g2f
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalWorld : TEXCOORD1;
			};

			v2g vert(appdata v)
			{
				v2g o;

				o.vertex = v.vertex;

				return o;
			}

			[maxvertexcount(92)]
			void geom(triangle v2g t[3], inout TriangleStream<g2f> ts)
			{
				float3 edgeA, edgeB;
				float3 normal;
				float4 p3;

				edgeA = t[1].vertex - t[0].vertex;
				edgeB = t[2].vertex - t[0].vertex;
				normal = normalize(cross(edgeA, edgeB));

				p3 = (t[0].vertex + t[1].vertex + t[2].vertex) / 3.0;

				// This is to make bottom slimmer, P3 now is D, not D'
				t[0].vertex = (p3 + t[0].vertex) / 2.0;
				t[1].vertex = (p3 + t[1].vertex) / 2.0;
				t[2].vertex = (p3 + t[2].vertex) / 2.0;

				float windIntensity = _WindIntensity * 0.25;

				float phi = abs(p3.x + p3.z) % 6.28;
				float omega = 1 + (phi / 6.28);
				float amp = windIntensity * (omega - 1);

				float movement = amp * sin(omega * _Time.w + phi);
				float h = (_Height+omega)/_Sections;

				g2f p0, p1, p2;

				float3 normalWorld;
				float3 center;

				float3 w;

				float4 next[3];

				for (uint s = 0; s < (_Sections-1); s++)
				{
					center = (t[0].vertex + t[1].vertex + t[2].vertex) / 3.0;
					
					next[0] = t[0].vertex + float4((center - t[0].vertex.xyz)/_Sections, 0) + float4(0, h, 0, 0);
					next[1] = t[1].vertex + float4((center - t[1].vertex.xyz)/_Sections, 0) + float4(0, h, 0, 0);
					next[2] = t[2].vertex + float4((center - t[2].vertex.xyz)/_Sections, 0) + float4(0, h, 0, 0);

					w = t[0].vertex.y * windIntensity * _WindDirection.xyz * (1 + movement); w.y = 0;

					next[0] += float4(w, 0);
					next[1] += float4(w, 0);
					next[2] += float4(w, 0);

					for (uint i = 0; i < 3; i++)
					{
						p0.posWorld = mul(unity_ObjectToWorld, t[i%3].vertex);
						p0.pos = mul(UNITY_MATRIX_MVP, t[i%3].vertex);

						p1.posWorld = mul(unity_ObjectToWorld, next[(i+1)%3]);
						p1.pos = mul(UNITY_MATRIX_MVP, next[(i+1)%3]);

						p2.posWorld = mul(unity_ObjectToWorld, next[i%3]);
						p2.pos = mul(UNITY_MATRIX_MVP, next[i%3]);

						normalWorld = normalize(cross(p2.posWorld - p0.posWorld, p1.posWorld - p0.posWorld));

						p0.normalWorld = normalWorld;
						p1.normalWorld = normalWorld;
						p2.normalWorld = normalWorld;

						ts.Append(p0);
						ts.Append(p1);
						ts.Append(p2);

						ts.RestartStrip();


						p0.posWorld = mul(unity_ObjectToWorld, t[i%3].vertex);
						p0.pos = mul(UNITY_MATRIX_MVP, t[i%3].vertex);

						p1.posWorld = mul(unity_ObjectToWorld, t[(i+1)%3].vertex);
						p1.pos = mul(UNITY_MATRIX_MVP, t[(i+1)%3].vertex);

						p2.posWorld = mul(unity_ObjectToWorld, next[(i+1)%3]);
						p2.pos = mul(UNITY_MATRIX_MVP, next[(i+1)%3]);

						normalWorld = normalize(cross(p2.posWorld - p0.posWorld, p1.posWorld - p0.posWorld));

						p0.normalWorld = normalWorld;
						p1.normalWorld = normalWorld;
						p2.normalWorld = normalWorld;

						ts.Append(p0);
						ts.Append(p1);
						ts.Append(p2);

						ts.RestartStrip();
					}

					t[0].vertex = next[0];
					t[1].vertex = next[1];
					t[2].vertex = next[2];
				}

				w = t[0].vertex.y * windIntensity * _WindDirection.xyz * (1 + movement); w.y = 0;

				p3 = ((t[0].vertex + t[1].vertex + t[2].vertex) / 3.0) + float4(0, h, 0, 0);
				p3 += float4(w, 0);

				for (uint j = 0; j < 3; j++)
				{
					p0.posWorld = mul(unity_ObjectToWorld, t[j%3].vertex);
					p0.pos = mul(UNITY_MATRIX_MVP, t[j%3].vertex);

					p1.posWorld = mul(unity_ObjectToWorld, t[(j+1)%3].vertex);
					p1.pos = mul(UNITY_MATRIX_MVP, t[(j+1)%3].vertex);

					p2.posWorld = mul(unity_ObjectToWorld, p3);
					p2.pos = mul(UNITY_MATRIX_MVP, p3);

					normalWorld = normalize(cross(p2.posWorld - p0.posWorld, p1.posWorld - p0.posWorld));

					p0.normalWorld = normalWorld;
					p1.normalWorld = normalWorld;
					p2.normalWorld = normalWorld;

					ts.Append(p0);
					ts.Append(p1);
					ts.Append(p2);

					ts.RestartStrip();
				}
			}


			half4 frag(g2f i) : COLOR
			{
				float3 normal, light;

				if(_WorldSpaceLightPos0.w == 0.0)
				{
					light = normalize(_WorldSpaceLightPos0.xyz);
				}
				else
				{
					light = normalize(_WorldSpaceLightPos0.xyz - i.posWorld.xyz);
				}

				normal = normalize(i.normalWorld);

				return _LightColor0 * _Color * (0.5 + 0.5 * saturate(dot(normal, light)));
			}

			ENDCG
		}
		
	}

}
