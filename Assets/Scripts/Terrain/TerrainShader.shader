Shader "Custom/TerrainShader"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxColoursCount = 8;
		const static float epsilon = 0.0001;

		int baseColoursCount;
		float3 baseColor;
		float3 baseColours[maxColoursCount];
		float baseStartHeights[maxColoursCount];
		float baseBlends[maxColoursCount];

		float minHeight;
		float maxHeight;
        struct Input
        {
			float3 worldPos;
        };

		float inverseLerp(float a, float b, float value)
		{
			return saturate((value - a) / (b - a));
		}
		 
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
			o.Albedo = baseColor;
			for (int i = 0; i < baseColoursCount; i++)
			{
				float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);
				o.Albedo = o.Albedo * (1 - drawStrength) + baseColours[i] * drawStrength;
			}
        }
        ENDCG
    }
    FallBack "Diffuse"
}
