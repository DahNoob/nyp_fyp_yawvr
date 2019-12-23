#ifndef SHIELD_HITS
#define SHIELD_HITS

//Number of hits
int m_hitCount = 0;
//Array of radiuses
float m_hitRadius[10];
//Array of object positions
float3 m_hitObjectPosition[10];
//Intensities of arrays
float m_hitIntensity[10];
//Ring widths
float m_hitWidth[10];
//Crack intensity
float m_crackStrength;

float DrawRing(float intensity, float radius, float dist, float width)
{
	float border = width;
	float currentRadius = lerp(0, radius, 1 - intensity);//expand radius over time 
	return intensity * (1 - smoothstep(currentRadius, currentRadius + border, dist) - (1 - smoothstep(currentRadius - border, currentRadius, dist)));
}

float DrawSharpRing(float intensity, float radius, float dist, float width, float power)
{
	float border = width;
	float currentRadius = lerp(0, radius, 1 - intensity);//expand radius over time 
	return pow((intensity * (1 - lerp(currentRadius, currentRadius + border, dist) - (1 - lerp(currentRadius - border, currentRadius, dist)))), power);
}

void CalculateHitsFactor_float(float3 objectPosition, out float factor)
{
	factor = 0;
	for (int i = 0; i < m_hitCount; i++)
	{
		float distanceToHit = distance(objectPosition, m_hitObjectPosition[i]);
		factor += DrawRing(m_hitIntensity[i], m_hitRadius[i], distanceToHit, m_hitWidth[i]);
	}
	factor = saturate(factor);
}

void CalculateHitsFactor2_float(float3 objectPosition, out float factor)
{
	factor = 0;
	for (int i = 0; i < m_hitCount; i++)
	{
		float distanceToHit = distance(objectPosition, m_hitObjectPosition[i]);
		factor += DrawSharpRing(m_hitIntensity[i], m_hitRadius[i], distanceToHit, m_hitWidth[i], 5);
	}
	factor = saturate(factor);
}

void CrackStrength_float(out float crackStrength)
{
	crackStrength = m_crackStrength;
}
#endif
