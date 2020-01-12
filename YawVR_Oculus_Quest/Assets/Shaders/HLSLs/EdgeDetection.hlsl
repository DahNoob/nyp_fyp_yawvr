#ifndef EDGE_DETECTION
#define EDGE_DETECTION

TEXTURE2D(_CameraColorTexture);
SAMPLER(sampler_CameraColorTexture);
float4 _CameraColorTexture_TexelSize;

//Number of hits
int m_scanCount = 0
//Array of object positions
float3 m_scanPositions[10];
//Array of scan distances;
float m_scanDistances[10];
//Array of scan widths;
float m_scanWidths[10];

//Expensive so yikes
float sobel(float2 UV)
{
	float3 TL = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV + float2(-1, 1) * 0.001);
	float3 TM = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV + float2(0, 1)* 0.001);
	float3 TR = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV + float2(1, 1)* 0.001);

	float3 ML = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV + float2(-1, 0) * 0.001);
	float3 MR = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV + float2(1, 0)* 0.001);

	float3 BL = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV + float2(-1, -1)* 0.001);
	float3 BM = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV + float2(0, -1)* 0.001);
	float3 BR = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV + float2(1, -1)* 0.001);

	float3 gradientX = -TL + TR - 2.0 * ML + 2.0 * MR - BL + BR;
	float3 gradientY = TL + 2.0 * TM + TR - BL - 2.0 * BM - BR;

	return sqrt(gradientX * gradientX + gradientY * gradientY);
}

void DoWave_float(
	float2 UV,
	float3 ScannerPosition,
	float3 WorldPos,
	float ScanDistance,
	float ScanWidth,
	float linearDepth,
	float LeadSharp,
	float3 MidColor,
	float3 LeadColor,
	float3 TrailColor,
	out float3 Out)
{
	float4 original = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV);
	float3 scannerCol = half4(0, 0, 0, 0);
	float dist = distance(WorldPos, ScannerPosition);

	if (dist < ScanDistance && dist > ScanDistance - ScanWidth && linearDepth < 1)
	{
		float diff = 1 - (ScanDistance - dist) / (ScanWidth);
		float3 edge = lerp(MidColor, LeadColor, pow(diff, LeadSharp));
		scannerCol = lerp(TrailColor, edge, diff);
		scannerCol *= diff;
	//float s = sobel(UV) * _EdgeDetectionStrength;
		float s = sobel(UV) * 5;
		//half4 resultSobel = half4(s * _EdgeTint.r, s * _EdgeTint.g, s * _EdgeTint.b, 1);
		half4 resultSobel = half4(s * MidColor.r, s * MidColor.g, s * MidColor.b, 1);
		scannerCol += resultSobel;
	}
	
	Out = original + scannerCol;
}

#endif
