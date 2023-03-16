using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class PerlinNoise
{
	public static float[,] GenerateNoiseMap(int width, int height, float scale,
		int octaves, float persistance, float lacunarity,
		int seed, Vector2 offset)
	{
		System.Random rand = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves]; // coordinates of the center point of each octave
		for (int o = 0; o < octaves; ++o)
		{
			octaveOffsets[o] = new Vector2(
				rand.Next(-100000, +100000),
				rand.Next(-100000, +100000));
		}

		scale = Mathf.Max(scale, 0.1f);
		float minNoise = float.MaxValue;
		float maxNoise = float.MinValue;
		float[,] noiseMap = new float[width, height];

		for(int y = 0 ; y < height; y++)
		{
			for(int x = 0; x < width; x++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noise = 0;
				for(int o = 0; o < octaves; ++o)
				{
					float scaledX = ((width - 1) / 2.0f - x) / scale;
					float scaledY = ((height - 1) / 2.0f - y) / scale;
					float sampleX = octaveOffsets[o].x + frequency * (offset.x * width / 1000 - scaledX);
					float sampleY = octaveOffsets[o].y + frequency * (offset.y * height / 1000 - scaledY);
					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
					perlinValue *= amplitude;
					noise += perlinValue;

					amplitude *= persistance;
					frequency *= lacunarity;
				}
				noiseMap[x, y] = noise;
				minNoise = Mathf.Min(noise, minNoise);
				maxNoise = Mathf.Max(noise, maxNoise);
			}
		}

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				noiseMap[x, y] = Mathf.InverseLerp(minNoise, maxNoise, noiseMap[x, y]);
			}
		}

		return noiseMap;
	}
}
