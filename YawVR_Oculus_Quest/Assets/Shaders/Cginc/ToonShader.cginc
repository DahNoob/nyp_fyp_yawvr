#ifndef TOON_SHADER
#define TOON_SHADER

void GetLightInformation_float(float3 ObjPos, out float3 Direction, out float ShadowAttenuation, out float3 Color)
{
#ifdef LIGHTWEIGHT_LIGHTING_INCLUDED
	//Actual light data from pipeline
	Light light = GetMainLight(GetShadowCoord(GetVertexPositionInputs(ObjPos)));
	Direction = light.direction;
	Color = light.color;
	ShadowAttenuation = light.shadowAttenuation;
#else
	//Hardcode data used for preview shader inside graph
	//Light functions are not available
	Direction = float3(-0.5, 0.5, -0.5);
	Color = float3(1, 1, 1);
	ShadowAttenuation = 0.4;
#endif
}

void GetLightInformationTwo_float(out float3 Direction, out float ShadowAttenuation, out float3 Color)
{
	Direction = float3 (0, 0, 0);
	Color = float3 (0, 0, 0);
	ShadowAttenuation = 0;

#ifdef LIGHTWEIGHT_LIGHTING_INCLUDED

	Light mainLight = GetMainLight();
	Color = mainLight.color;
	Direction = mainLight.direction;
	float4 shadowCoord;
#ifdef _SHADOWS_ENABLED
#if SHADOWS_SCREEN
	float4 clipPos = TransformWorldToHClip(WorldPos);
	shadowCoord = ComputeShadowCoord(clipPos);
#else
	shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
	mainLight.attenuation = MainLightRealtimeShadowAttenuation(shadowCoord);
#endif
	ShadowAttenuation = mainLight.shadowAttenuation;
#else
	//Hardcode data used for preview shader inside graph
	//Light functions are not available
	Direction = float3(-0.5, 0.5, -0.5);
	Color = float3(1, 1, 1);
	ShadowAttenuation = 0.4;
#endif


}


#endif
