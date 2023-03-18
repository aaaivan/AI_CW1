using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerrainData : ScriptableObject
{
	[Range(2, 255)] public int mapSize = 50;
	[Range(2.0f, 200.0f)] public float noiseScale = 100f;
	[Range(1, 3)] public int octaves = 2;
	[Range(0.0f, 1.0f)] public float persistance = 0.5f;
	[Range(1.0f, 9.99f)] public float lacunarity = 2f;
	public int seed = 1;
	public Vector2 offset = Vector2.zero;
	public float[,] noiseMap;

	[Range(0, 5.0f)] public float uniformScale = 1.0f;
	[Range(0.0f, 50.0f)] public float noiseHeightMultiplier = 100.0f;
	public bool useCurveMultiplier = true;
	public AnimationCurve meshHeightCurveMultiplier;
	public bool useFalloffMap = true;
	public float falloffSlope = 3;
	public float falloffDistance = 3;
	public float[,] falloffMap;

	public event System.Action OnValuesUpdated;

	public TerrainData()
	{
		falloffMap = FalloffGenerator.GenerateFalloutMap(mapSize, falloffSlope, falloffDistance);
	}

	public void GenerateNoiseMap()
	{
		noiseMap = PerlinNoise.GenerateNoiseMap(mapSize, mapSize, noiseScale,
												octaves, persistance, lacunarity,
												seed, offset);
	}

	public void NotifyValueUpdated()
	{
		if (OnValuesUpdated != null)
		{
			OnValuesUpdated();
		}
	}

	void OnValidate()
	{
		falloffMap = FalloffGenerator.GenerateFalloutMap(mapSize, falloffSlope, falloffDistance);
		if (mapSize < 2)
		{
			mapSize = 2;
		}
		if (mapSize > 255)
		{
			mapSize = 255;
		}
		if (lacunarity < 1)
		{
			lacunarity = 1;
		}
		if (octaves < 1)
		{
			octaves = 1;
		}
		if (noiseScale < 0)
		{
			noiseScale = 0;
		}
		if(uniformScale <= 0)
		{
			uniformScale = 0.01f;
		}
	}
}
