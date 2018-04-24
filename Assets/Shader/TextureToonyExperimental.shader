Shader "Custom/TextureToonyExperiment"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SpecularTex("Specular Map", 2D) = "black" {}
		_ShadowDispTex("Shadow Outline Tex", 2D) = "bump" {}
		_ShadowDispMap("Shadow Outline Map", 2D) = "black" {}
		_DarkerValue("DarkerValue", Range(0.15, .5)) = 0.5
		_SpecValue("SpeculaCutoff", Range(0.1, 1)) = 0.5
		_CutOff("CutOff", Range(0, 1)) = 0
		_PattrnCutOff("Pattern CutOff", Range(0, 0.5)) = 0
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
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

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
				float2 dispUv : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				SHADOW_COORDS(3)
				half3 tspace0 : TEXCOORD4; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD5; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD6; // tangent.z, bitangent.z, normal.z
				float4 pos : SV_POSITION;
				float3 normals : NORMAL;
			};

			sampler2D _MainTex;
			sampler2D _SpecularTex;
			sampler2D _ShadowDispMap;
			sampler2D _ShadowDispTex;
			float4 _ShadowDispTex_ST;
			float4 _MainTex_ST;

			float4 _Color;
			float4 _DarkColor;
			float _CutOff;
			float _PattrnCutOff;
			float _DarkerValue;
			float _SpecValue;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.dispUv = TRANSFORM_TEX(v.uv, _ShadowDispTex);

				half3 wNormal = UnityObjectToWorldNormal(v.normals);
				o.normals = wNormal;

				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				// output the tangent space matrix
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

				TRANSFER_SHADOW(o);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 specMap = tex2D(_SpecularTex, i.uv);
				fixed4 shadowMap = tex2D(_ShadowDispMap, i.uv);
				fixed shadow = SHADOW_ATTENUATION(i);

				float3 viewDirection = normalize(
					_WorldSpaceCameraPos - i.pos.xyz);

				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				//Specularity
				half3 h = normalize(lightDir + viewDirection);
				float nh = max(0, dot(i.normals, h));
				float spec = pow(nh, _SpecValue);
				half4 specCol = spec * half4(0.7, 0.7, 0.7, 255);

				//Dot product between normal and light direction
				float ndotL = max(0, dot(i.normals, lightDir));
				
				//Bump map normal
				half3 tnormal = UnpackNormal(tex2D(_ShadowDispTex, i.dispUv));
				half3 worldNormal;
				worldNormal.x = dot(i.tspace0, tnormal);
				worldNormal.y = dot(i.tspace1, tnormal);
				worldNormal.z = dot(i.tspace2, tnormal);

				//Calculate grayscale
				fixed4 grayVector = fixed4(0.3, 0.59, 0.11, 1);
				fixed x = dot(grayVector, col);
				fixed4 Intensity = fixed4(x, x, x, 1);

				//Calculated darkerColor
				fixed4 DarkColor = lerp(col, Intensity, .05) * _DarkerValue;

				//Decide if color is dark or normal
				float shadeDecider = step(ndotL, _CutOff);
				 
				//Decide between normal color or darkened and desaturated version of the color
				fixed4 baseCol = lerp(col, DarkColor, shadeDecider);
				fixed bumpdotL = max(0, dot(worldNormal, lightDir));

				//Apply normal to modify shading shape
				baseCol = lerp(baseCol, DarkColor, step(bumpdotL, _PattrnCutOff) * shadowMap.r);
				
				//Decide between specularColor and black depending on specular cutOff
				specCol = lerp(specCol, half4(0,0,0,0), step(nh, _SpecValue));

				//Decide between baseColor or casted shadow
				baseCol = lerp(baseCol, DarkColor, step(shadow, 0.7));

				//Apply specular mask
				baseCol = baseCol + specCol * specMap.r * step(_CutOff, ndotL);

				UNITY_APPLY_FOG(i.fogCoord, baseCol);

				return baseCol;
			}
			ENDCG
		}


		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
		//Fallback "Diffuse"
}
