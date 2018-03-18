Shader "Custom/TrueTimeFightZoneShaderDot"
{
	Properties
	{
		_Color("Color", Color) = (1,0,0,1)
		_DarkColor("Dark Color", Color) = (1,0,0,1)
		_CutOff("CutOff", Range(-1,1)) = 0
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		// make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float4 normals : NORMAL;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float3 normals : NORMAL;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _Color;
	float4 _DarkColor;
	float _CutOff;
	float4 _LightColor0;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		o.normals = mul(v.normals, unity_WorldToObject);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float4 ndotL = max(0, dot(i.normals, _WorldSpaceLightPos0.xyz));

		fixed4 col = ndotL;
		fixed4 baseCol = lerp(_Color, _DarkColor, step(ndotL, _CutOff));

		return baseCol;
	}
		ENDCG
	}
	}
}
