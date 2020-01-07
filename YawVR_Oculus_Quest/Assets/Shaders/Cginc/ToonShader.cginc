#ifndef TOON_SHADER
#define TOON_SHADER

void LWRPLightingFunction_float(float3 ObjPos, out float3 Direction, out float ShadowAttenuation, out float3 Color)
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
	Direction = float3(-0.5f, 0.5f, -0.5f);
	Color = float3(1, 1, 1);
	ShadowAttenuation = 0.4;
#endif
}


#endif
