Shader "Custom/TriplanarTerrain w Normals"
{
	Properties
	{
		//Textures
		_Color("Color", Color) = (1,1,1,1)										//Overall Color
		_MainTex("Main Texture", 2D) = "white" {}						// top texture
		_MainTexSide("Side Texture", 2D) = "white" {}					// right texture
		_NoiseTex("Noise Map", 2D) = "white" {}							//noise texture
		_MainOcclusionMap("Main Occlusion Map", 2D) = "white" {}		// main occulusion map
		_SideOcclusionMap("Side Occlusion Map", 2D) = "white" {}		// side occulusion map
		_MainSpecularMap("Main Specular Map", 2D) = "white" {}				// main specular map
		_SideSpecularMap("Side Specular Map", 2D) = "white" {}				// main specular map

		//Normal
		_MainNormal("Main Normal", 2D) = "bump" {}
		_SideNormal("Side Normal", 2D) = "bump" {}
			
		//Floats
		_EdgeWidth("Edge Width", Float) = 1								//edge width
		_MainTexScale("Main Texture Scale", Float) = 1			// top texture scale
		_MainTexSideScale("Side Texture Scale", Float) = 1		// right texture scale 
		_NoiseScale("Noise Texture Scale", Float) = 1				// right texture scale 
		_MainSpecularMapScale("Main Specular Map Scale", Float) = 1								// top texture scale
		_SideSpecularMapScale("Side Specular Map Scale", Float) = 1							// right texture scale 
		_MainOcculusionMapScale("Main Occulusion Map Scale", Float) = 1									// right texture scale 
		_SideOcculusionMapScale("Side Occulusion Map Scale", Float) = 1									// right texture scale 
		_MainNormalMapScale("Main Normal Map Scale", Float) = 1									// right texture scale 
		_SideNormalMapScale("Side Normal Map Scale", Float) = 1
		_TopSpread("TopSpread", Range(-2,2)) = 1					//top spread
		_RimPower("Rim Power", Range(0,20)) = 10					//Rim brightness
		_FinalResultBrightness("Final Brightness", Range(1,5)) = 1

		//Colors
		_EdgeColor("Edge Color", Color) = (1,1,1,1)					//edge color
		_RimColor("Top Rim Color" , Color) = (1,1,1,1)				//Rim Color for Top Tex
		_RimColorSide("Side Rim Color" , Color) = (1,1,1,1)		//Rim Color for Side Tex

	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf StandardSpecular fullforwardshadows 

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

		sampler2D _MainTex, _MainTexSide, _NoiseTex , _MainNormal, _SideNormal;
		sampler2D _MainOcclusionMap, _SideOcclusionMap; // occulusions
		sampler2D _MainSpecularMap, _SideSpecularMap; //speculars
		float _MainTexScale, _MainTexSideScale, _NoiseScale;
		float _MainSpecularMapScale, _SideSpecularMapScale, _MainOcclusionMapScale, _SideOcclusionMapScale;
		float _MainNormalMapScale, _SideNormalMapScale;
		float _TopSpread, _EdgeWidth, _RimPower;
		float4 _EdgeColor , _RimColor, _RimColorSide;
		float _FinalResultBrightness;

		struct Input
		{
			//uv of mainTexture linked to properties
			float2 uv_MainTex : TEXCOORD0;
			//View direction for rim lighting
			float3 viewDir;
			//World position
			float3 worldPos; 
			INTERNAL_DATA
			//World normal;
			float3 worldNormal;

			};
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
				UNITY_INSTANCING_BUFFER_END(Props)


			float3 ReturnCalculatedTexture(sampler2D _textureFile, float _textureScale, float3 blendNormal, float3 worldPosition)
			{
				float3 x = tex2D(_textureFile, worldPosition.yz *_textureScale);
				float3 y = tex2D(_textureFile, worldPosition.xz *_textureScale);
				float3 z = tex2D(_textureFile, worldPosition.xy *_textureScale);

				//Lerp together
				float3 resultTexture = z;
				resultTexture = lerp(resultTexture, x, blendNormal.x);
				resultTexture = lerp(resultTexture, y, blendNormal.y);

				return resultTexture;
			}

			float3 ReturnNormals(sampler2D _textureFile, float _textureScale, float3 blendNormal, float3 worldPosition)
			{
				//Get normal for top texture
				float3 xN = UnpackNormal(tex2D(_textureFile, worldPosition.zy *_textureScale));
				float3 yN = UnpackNormal(tex2D(_textureFile, worldPosition.zx *_textureScale));
				float3 zN = UnpackNormal(tex2D(_textureFile, worldPosition.xy *_textureScale));

				//Lerp together
				float3 resultNormal = zN;
				resultNormal = lerp(resultNormal, xN, blendNormal.x);
				resultNormal = lerp(resultNormal, yN, blendNormal.y);

				return resultNormal;
			}

			void surf(Input IN, inout SurfaceOutputStandardSpecular o)
			{
				//Clamp and increase power to use as a blend between the projected textures
				float3 worldNormalVector = WorldNormalVector(IN, o.Normal);
				//Blend normals
				float3 blendNormal = saturate(pow(worldNormalVector * 1.4, 4));

				//Textures
				float3 mainTexture = ReturnCalculatedTexture(_MainTex, _MainTexScale, blendNormal, IN.worldPos);
				float3 sideTexture = ReturnCalculatedTexture(_MainTexSide, _MainTexSideScale, blendNormal, IN.worldPos);
				float3 noiseTexture = ReturnCalculatedTexture(_NoiseTex, _NoiseScale, blendNormal, IN.worldPos);

				//Normal maps
				float3 mainNormal = ReturnNormals(_MainNormal, _MainNormalMapScale, blendNormal, IN.worldPos);
				float3 sideNormal = ReturnNormals(_SideNormal, _SideNormalMapScale, blendNormal, IN.worldPos);

				//Speculars
				float3 specularTexture = ReturnCalculatedTexture(_MainSpecularMap, _MainSpecularMapScale, blendNormal, IN.worldPos);
				float3 sideSpecularTexture = ReturnCalculatedTexture(_SideSpecularMap, _SideSpecularMapScale, blendNormal, IN.worldPos);

				//Occulusion
				float3 occulusionTexture = ReturnCalculatedTexture(_MainOcclusionMap, _MainOcclusionMapScale, blendNormal, IN.worldPos);
				float3 sideOcculusionTexture = ReturnCalculatedTexture(_SideOcclusionMap, _MainOcclusionMapScale, blendNormal, IN.worldPos);

				//Rim for main
				half mainRim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal * noiseTexture));

				half sideRim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

				//Dot product of world normal and surface normal
				float worldNormalDot = dot(o.Normal + (noiseTexture.y + (noiseTexture * 0.5)), worldNormalVector.y);

				//Texture results.
				float3 topTextureResult = step(_TopSpread, worldNormalDot) * mainTexture;
				float3 sideTextureResult = step(worldNormalDot, _TopSpread) * sideTexture;

				//Normal results
				float3 topNormalResult = step(_TopSpread, worldNormalDot) * mainNormal;
				float3 sideNormalResult = step(worldNormalDot, _TopSpread) * sideNormal;

				float3 topTextureEdgeResult = (step(_TopSpread, worldNormalDot)
																	* step(worldNormalDot, _TopSpread + _EdgeWidth))
																	* _EdgeColor;

				//Calculation of speculars
				float3 topSpecularResult = step(_TopSpread, worldNormalDot) * specularTexture;
				float3 sideSpecularResult = step(worldNormalDot, _TopSpread) * sideSpecularTexture;

				//Calculation of occulusions
				float3 topOcclusionResult = step(_TopSpread, worldNormalDot) * occulusionTexture;
				float3 sideOcclusionResult = step(worldNormalDot, _TopSpread) * sideOcculusionTexture;

				//Final normals
				o.Normal = topNormalResult + sideNormalResult;

				//Albedo color
				o.Albedo = (topTextureResult + sideTextureResult + topTextureEdgeResult) * _FinalResultBrightness  * _Color;

				//Adding rimlight and comibning both top and side
				o.Emission = step(_TopSpread, worldNormalDot) *_RimColor.rgb * pow(mainRim, _RimPower) +
									step(worldNormalDot, _TopSpread) * _RimColorSide.rgb * pow(sideRim, _RimPower);

				//Add specular.
				o.Specular = topSpecularResult + sideSpecularResult;

				//Set occulusion
				o.Occlusion = (topOcclusionResult + sideOcclusionResult).r;

			}

			ENDCG
		}
			FallBack "Diffuse"
}
