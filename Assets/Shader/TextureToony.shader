Shader "Custom/TextureToony"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SpecularTex("Specular Map", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_DarkerValue("DarkerValue", Range(0.15, .5)) = 0.5
		_CutOff("CutOff", Range(0,1)) = 0
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
		LOD 100
		ZWrite On

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
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				// that transforms from tangent to world space
				half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
			};

			sampler2D _MainTex;
			sampler2D _SpecularTex;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			float4 _Color;
			float4 _DarkColor;
			float _CutOff;
			float _DarkerValue;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _BumpMap);

				half3 wNormal = UnityObjectToWorldNormal(v.normals);
				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				// output the tangent space matrix
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 specMap = tex2D(_SpecularTex, i.uv);
				half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));


				// transform normal from tangent to world space
				half3 worldNormal;
				worldNormal.x = dot(i.tspace0, tnormal);
				worldNormal.y = dot(i.tspace1, tnormal);
				worldNormal.z = dot(i.tspace2, tnormal);

				float ndotL = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				
				//Calculate grayscale
				fixed4 grayVector = fixed4(0.3, 0.59, 0.11, 1);
				fixed x = dot(grayVector, col);
				fixed4 Intensity = fixed4(x, x, x, 1);

				//Decide between normal color or darkened and desaturated version of the color
				fixed4 baseCol = lerp(col, lerp(col, Intensity, .05) * _DarkerValue, step(ndotL, _CutOff));

				UNITY_APPLY_FOG(i.fogCoord, baseCol);

				return baseCol;
			}
			ENDCG
		}
	}
		//Fallback "Diffuse"
}
