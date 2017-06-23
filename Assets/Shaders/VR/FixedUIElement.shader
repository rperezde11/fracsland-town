Shader "Custom/VR/FixedUIElement" {
	
	Properties {
		_MainTex("UI Texture", 2D) = "white"{}
		_Size("Element Size", Range(0, 1)) = 0.5
		_X("Screen X Position", Range(0,1)) = 0
		_Y("Screen Y Position", Range(0, 1)) = 0
	}

	SubShader
	{
		Tags{ "Queue" = "Overlay+2" }

		Pass
		{
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "../includes/Common.cginc"

			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;
			uniform float _Size;
			uniform float _X;
			uniform float _Y;

			struct appdata
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos  : SV_POSITION;
				float2 uv   : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;

				o.pos = float4(
					2.0 * (_X + _Size * v.vertex.x) - 1.0,
					_ProjectionParams.x * (2.0 * (_Y + _Size * v.vertex.z) - 1.0),
					_ProjectionParams.y,
					1.0
				);

				o.uv = v.texcoord;

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				half4 finalColor;

				finalColor = tex2D(_MainTex, i.uv);

				return finalColor;
			}

			ENDCG
		}
	}
}
