Shader "Hidden/EdgeDetection"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScanDistance("Scan Distance", Float) = 10
		_ScanWidth("Scan Width", Float) = 5
		_EdgeDetectionStrength("Edge Brightness", Float) = 0.5
		_EdgeTint("Tint Color", Color) = (1,1,1,1)
		_LeadSharp("Edge Sharpness", float) = 10
		_LeadColor("Edge Color", Color) = (1, 1, 1, 0)
		_MidColor("Mid Color", Color) = (1, 1, 1, 0)
		_TrailColor("Trail Color", Color) = (1, 1, 1, 0)
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct VertIn
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float4 ray : TEXCOORD1;
				};

				struct VertOut
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float2 uv_depth : TEXCOORD1;
					float4 interpolatedRay : TEXCOORD2;
				};

				uniform float4 _MainTex_TexelSize;

				VertOut vert(VertIn v)
				{
					VertOut o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.uv_depth = v.uv;

					//Inverse Y
	#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						o.uv.y + 1 - o.uv.y;
	#endif

					o.interpolatedRay = v.ray;
					return o;
				}

				sampler2D _MainTex, _DetailTex;
				sampler2D_float _CameraDepthTexture;
				//Size of main texture

				float4 _ScanOrigin;
				float _ScanDistance , _ScanWidth, _EdgeDetectionStrength;
				float4 _EdgeTint;
				float _LeadSharp;
				float4 _LeadColor, _MidColor, _TrailColor, _HBarColor;
				float _LinearDepth;

				float sobel(sampler2D mainTex, float2 uv)
				{
					float3 TL = tex2D(mainTex, uv + float2(-1, 1) * 0.001);
					float3 TM = tex2D(mainTex, uv + float2(0, 1)* 0.001);
					float3 TR = tex2D(mainTex, uv + float2(1, 1)* 0.001);

					float3 ML = tex2D(mainTex, uv + float2(-1, 0) * 0.001);
					float3 MR = tex2D(mainTex, uv + float2(1, 0)* 0.001);

					float3 BL = tex2D(mainTex, uv + float2(-1, -1)* 0.001);
					float3 BM = tex2D(mainTex, uv + float2(0, -1)* 0.001);
					float3 BR = tex2D(mainTex, uv + float2(1, -1)* 0.001);

					float3 gradientX = -TL + TR - 2.0 * ML + 2.0 * MR - BL + BR;
					float3 gradientY = TL + 2.0 * TM + TR - BL - 2.0 * BM - BR;

					return sqrt(gradientX * gradientX + gradientY * gradientY);
				}
					half4 frag(VertOut i) : SV_Target
					{
						half4 col = tex2D(_MainTex, i.uv);

						//Calculate the depth
					float rawDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv_depth));
					//Give value between 0 and 1
					float linearDepth = Linear01Depth(rawDepth);
					//Direction of ray
					float4 worldSpaceDirection = linearDepth * i.interpolatedRay;
					//Calculated world space
					float3 worldSpace = _ScanOrigin + worldSpaceDirection;

					//Distance towards origin from point to origin
					float distanceToOrigin = distance(worldSpace, _ScanOrigin);

					half4 scannerColor = half4(0, 0, 0, 0);

					//if it exceeds 1 it has went past the far plane
					if (distanceToOrigin < _ScanDistance && distanceToOrigin > _ScanDistance - _ScanWidth && linearDepth < 1)
					{
						//Distance function?
						float difference = 1 - (_ScanDistance - distanceToOrigin) / (_ScanWidth);
						half4 edge = lerp(_MidColor, _LeadColor, pow(difference, _LeadSharp));
						//Make it more transparent towards the edge
						scannerColor = lerp(_TrailColor, edge, difference);
						scannerColor *= difference;
						float s = sobel(_MainTex, i.uv) * _EdgeDetectionStrength;
						//Get the final result of sobel * edgeTint.
						half4 resultSobel = half4(s * _EdgeTint.r, s * _EdgeTint.g, s * _EdgeTint.b, 1);
						//Add the sobel result to the scan color such that it only shows the sobel in the scan
						scannerColor += resultSobel;
					}

					if (distanceToOrigin < _ScanDistance)
					{
						return col + scannerColor;
					}

					if (linearDepth >= 1)
						return col;
					else
						return col * 0.1f;
					}


				ENDCG
			}
		}
}
