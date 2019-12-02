Shader "Project/Holograph" {
	Properties
	{
	  _InnerColor("Inner Color", Color) = (1.0, 1.0, 1.0, 1.0)
	  _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	  _RimArea("Rim Area", Range(0.5,8.0)) = 3.0
	  _RimPower("Rim Power", Range(1.0, 20.0)) = 1.0
	  _RimFadeRate("Rim Fade Rate", Range(0.0, 10.0)) = 1.0
	  _SkinTexture("Skin Texture", 2D) = "" {}
	  _SkinSpeed("Skin Speed", Range(0.0, 10.0)) = 1.0
	  _SkinAlpha("Skin Alpha", Range(0.0, 1.0)) = 0.5
	}
		SubShader
	{
	  Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

	  Cull Back
	  Blend One One

	  CGPROGRAM
	  #pragma surface surf Lambert

	  struct Input
	  {
		  float3 viewDir;
		  float2 uv_SkinTexture;
		  float4 screenPos;
	  };

	  float4 _InnerColor;
	  float4 _RimColor;
	  float _RimArea;
	  float _RimPower;
	  float _RimFadeRate;
	  float _SkinSpeed;
	  float _SkinAlpha;
	  sampler2D _SkinTexture;

	  float alphaGen(float _seed)
	  {
		  return (cos(_seed) + 1.0) * 0.5;
	  }

	  void surf(Input IN, inout SurfaceOutput o)
	  {
		  float2 skin_uv_offset = fmod(_Time.y * _SkinSpeed, 2.0) - 1.0;//float2(_Time.y, _Time.y) * _SkinSpeed;
		  float4 skin_tex = tex2D(_SkinTexture, IN.uv_SkinTexture + skin_uv_offset) * _SkinAlpha;
		  float rim_color_alpha = (cos(_Time.y * _RimFadeRate) + 1.0) * 0.5 + (skin_tex.a);//(_CosTime.a * _RimFadeRate) + 1.0;
		  float4 rim_color = _RimColor * rim_color_alpha; //_RimColor * float4(rim_color_alpha, rim_color_alpha, rim_color_alpha, rim_color_alpha);
		  //o.Albedo = _InnerColor;
		  //float4 tex = tex2D(_SkinTexture, IN.uv_SkinTexture);
		  o.Albedo = _InnerColor;
		  o.Albedo *= skin_tex;
		  //float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
		  //screenUV *= float2(8, 6) * (1 + _Time.y * _SkinSpeed);
		  //o.Albedo *= tex2D(_SkinTexture, screenUV).rgb * 2;
		  //o.Alpha = tex.a * _SkinAlpha;
		  half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
		  o.Emission = rim_color * pow(rim, _RimArea) * _RimPower;
		  //o.Alpha = ;
		  //o.Emission += tex2D(_SkinTexture, IN.uv_MainTex).rgb;
	  }
	  ENDCG
	}
		Fallback "Diffuse"
}
