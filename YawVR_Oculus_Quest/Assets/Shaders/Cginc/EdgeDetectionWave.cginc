#ifndef EDGE_DETECTION_WAVE
#define EDGE_DETECTION_WAVE

TEXTURE2D(_CameraColorTexture);
SAMPLER(sampler_CameraColorTexture);
float4 _CameraColorTexture_TexelSize;

void DoWave_float(
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
	float3 scannerCol = half4(0, 0, 0, 0);
	float dist = distance(WorldPos, ScannerPosition);

	if (dist < ScanDistance && dist > ScanDistance - ScanWidth && linearDepth < 1)
	{
		float diff = 1 - (ScanDistance - dist) / (ScanWidth);
		float3 edge = lerp(MidColor, LeadColor, pow(diff, LeadSharp));
		scannerCol = lerp(TrailColor, edge, diff);
		scannerCol *= diff;
	}
	Out = scannerCol;
}


#endif
