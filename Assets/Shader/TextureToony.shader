﻿Shader "Custom/TextureToony"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SpecularTex("Specular Map", 2D) = "black" {}
		_ShadowDispMap("Shadow Outline Displacement", 2D) = "white" {}
		_DarkerValue("DarkerValue", Range(0.15, .5)) = 0.5
		_SpecValue("SpeculaCutoff", Range(0.1, 1)) = 0.5
		_CutOff("CutOff", Range(0,1)) = 0
		_PattrnCutOff("Pattern CutOff", Range(0,1)) = 0
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
				float4 pos : SV_POSITION;
				float3 normals : NORMAL;
			};

			sampler2D _MainTex;
			sampler2D _SpecularTex;
			sampler2D _ShadowDispMap;
			float4 _ShadowDispMap_ST;
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
				o.dispUv = TRANSFORM_TEX(v.uv, _ShadowDispMap);

				half3 wNormal = UnityObjectToWorldNormal(v.normals);
				o.normals = wNormal;

				TRANSFER_SHADOW(o);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 specMap = tex2D(_SpecularTex, i.uv);
				fixed shadow = SHADOW_ATTENUATION(i);

				float3 viewDirection = normalize(
					_WorldSpaceCameraPos - i.pos.xyz);

				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				//Specularity
				half3 h = normalize(lightDir + viewDirection);
				float nh = max(0, dot(i.normals, h));
				float spec = pow(nh, _SpecValue);
				half4 specCol = spec * half4(0.7, 0.7, 0.7, 255);

				float ndotL = max(0, dot(i.normals, lightDir));
				
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

				float uShadowDisp = clamp((ndotL - _PattrnCutOff) / _CutOff, 0, 1);
				fixed4 shadowDispl = tex2D(_ShadowDispMap, half2(i.dispUv.x, i.dispUv.y));
				baseCol = lerp(baseCol, DarkColor, step(ndotL, _CutOff - _PattrnCutOff) * shadowDispl.r);
				
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
