Shader "Custom/Nature/OldWater" {

	Properties{
		_BumpTex("Distortion Texture", 2D) = "bump"{}
		_Color("Color", Color) = (1,1,1,1)
		_Speed("Speed", Range(0,1)) = 0.5
		_Distortion("Distortion Factor", Range(0,1)) = 0.5
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" }

		// This pass grabs the screen behind
		// accessible on next pass as _GrabPass
		GrabPass{

	}

	Pass
	{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag

#pragma multi_compile _REFLECTION_PERFECT

#include "../includes/Common.cginc"
#include "../includes/UtilsBump.cginc"
#include "../includes/UtilsReflectivity.cginc"

		uniform sampler2D _BumpTex;
	uniform half4 _BumpTex_ST;
	uniform sampler2D _GrabTexture;
	uniform half4 _GrabTexture_ST;
	uniform half4 _Color;
	uniform float _Speed;
	uniform float _Distortion;

	uniform half4 _LightColor0;

	struct appdata
	{
		float4 vertex   : POSITION;
		float3 normal   : NORMAL;
		float4 tangent  : TANGENT;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos         : SV_POSITION;
		float4 posWorld    : TEXCOORD0;
		float3 normalWorld : TEXCOORD1;
		float2 uv          : TEXCOORD2;
		float4 uv_Grab     : TEXCOORD3;

		BUMP_COORDS(4,5)
	};

	v2f vert(appdata v)
	{
		v2f o;

		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		o.normalWorld = mul(v.normal, unity_WorldToObject);
		o.uv = v.texcoord;
		o.uv_Grab = ComputeGrabScreenPos(o.pos);

		TRANSFER_BUMP(o);

		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		float attenuation;
	float3 light, view, halff;

	InitFragmentVectors(i.posWorld, i.normalWorld, light, view, halff, attenuation);

	float3 displacement = UnpackNormals(_BumpTex, (i.uv * _BumpTex_ST.xy + _BumpTex_ST.zw + _Time.x * _Speed) , _Distortion);

	half3x3 localToWorldTranspose = half3x3(
		normalize(i._TangentWorld),
		normalize(i._BinormalWorld),
		normalize(i.normalWorld)
		);

	i.normalWorld = normalize(mul(displacement, localToWorldTranspose));

	float3 r = ComputeReflexDirection(view, i.normalWorld, i.posWorld);
	half4 color = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, r, 0.1 * 6);

	i.uv_Grab.xy += displacement.xy;
	half4 color2 = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uv_Grab)) * _LightColor0 * _Color;

	color = lerp(color, color2, saturate(displacement.x));

	return color2;
	}

		ENDCG
	}

	}
}
