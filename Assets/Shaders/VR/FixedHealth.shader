Shader "Custom/VR/FixedHealth" {
	
	Properties {
		_MainTex("UI Texture", 2D) = "white"{}
		_ColorFull("Color Full", Color) = (1,0,0,1)
		_ColorEmpty("Color Empty", Color) = (1,0,0,1)
		_Visibility("Visibility", Range(0,1)) = 0.75
		_Size("Element Size", Range(0, 1)) = 0.5
		_X("Screen X Position", Range(0,1)) = 0
		_Y("Screen Y Position", Range(0, 1)) = 0
		_Lives("Lives", Int) = 5
		_RemainingLives("Remaining", Int) = 3
	}

	SubShader
	{
		Tags{ "Queue" = "Overlay+2" }

		Pass
		{
			CGINCLUDE

			float getAngle(float2 vec2)
			{
				float cos, angle;
				float2 positive;

				positive.x = (vec2.x) > 0;
				positive.y = (vec2.y) > 0;

				// first quadrant
				if(positive.x && positive.y)
				{
					cos = dot(float2(1,0), vec2);
					angle = acos(cos)*57;
				}

				// second quadrant
				if (!positive.x && positive.y)
				{
					cos = dot(float2(0,1), vec2);
					angle = 90 + acos(cos)*57;
				}

				// third quadrant
				if (!positive.x && !positive.y)
				{
					cos = dot(float2(-1,0), vec2);
					angle = 180 + acos(cos)*57;
				}

				// fourth quadrant
				if (positive.x && !positive.y)
				{
					cos = dot(float2(0,-1), vec2);
					angle = 270 + acos(cos)*57;
				}

				return angle;
			}

			ENDCG

			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "../includes/Common.cginc"

			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;
			uniform half4 _ColorFull;
			uniform half4 _ColorEmpty;
			uniform float _Visibility;
			uniform float _Size;
			uniform float _X;
			uniform float _Y;
			uniform float _Lives;
			uniform float _RemainingLives;

			struct appdata
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos         : SV_POSITION;
				float2 uv          : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;

				o.pos = float4(
					2.0 * (_X + _Size * v.vertex.x) - 1.0,
					_ProjectionParams.x * (2.0 * (_Y + _Size * v.vertex.z) - 1.0) * (_ScreenParams.x/_ScreenParams.y),
					_ProjectionParams.y,
					1.0
				);

				// Texture has components inverted
				o.uv = 1 - v.texcoord;

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				half4 finalColor;

				float angle = getAngle(normalize(i.uv * 2 - 1));
				float step = 360 / _Lives;
				float stepTransitionBetweenSteps = step * 0.05;

				int numStep = floor(angle/step);
				float fromStep = angle % step;
				half mask = tex2D(_MainTex, i.uv).r;

				// mask
				finalColor = half4(1, 1, 1, _Visibility * (mask > 0.5));
				
				// spacing between steps
				finalColor *= mask * saturate(((fromStep - stepTransitionBetweenSteps) * ((step - fromStep) - stepTransitionBetweenSteps)));
				
				// full steps vs empty steps
				finalColor *= ((_RemainingLives*step) > (numStep*step)) ? _ColorFull : _ColorEmpty;

				return finalColor;
			}

			ENDCG
		}
	}
}
