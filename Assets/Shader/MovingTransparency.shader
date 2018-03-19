Shader "Custom/MovingTransparency"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DistTex("Distortion Text", 2D) = "white" {}
		_Color("Color", Color) = (1,0,0,1)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _DistTex;
			float4 _MainTex_ST;
			float4 _DistTex_ST;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _DistTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 distortion = tex2D(_DistTex, i.uv2);
				// sample the texture
				_Color.a = tex2D(_MainTex, i.uv + distortion.rg).a;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return _Color;
			}
			ENDCG
		}
	}
}
