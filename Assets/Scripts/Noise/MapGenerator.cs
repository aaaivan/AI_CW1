using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public enum DrawMode {NoiseMap, Colourmap, Mesh};
	[SerializeField]
	DrawMode drawMode = DrawMode.NoiseMap;

	[SerializeField]
	[Range(1, 1000)]
	int mapWidth = 1;
	[SerializeField]
	[Range(1, 1000)]
	int mapHeight = 1;
	[SerializeField]
	[Range(0.0f, 200.0f)]
	float noiseScale = 0.5f;
	[SerializeField]
	[Range(0.0f, 500.0f)]
	float noiseHeightMultiplier = 100.0f;
	[SerializeField]
	AnimationCurve meshHeightMultiplier;
	[SerializeField]
	[Range(1, 3)]
	int octaves = 2;
	[SerializeField]
	[Range(0.0f, 1.0f)]
	float persistance = 0.5f;
	[SerializeField]
	[Range(1.0f, 10.0f)]
	float lacunarity = 2f;
	[SerializeField]
	int seed = 1;
	[SerializeField]
	Vector2 offset = Vector2.zero;
	[SerializeField]
	TerrainType[] regions;

	public void GenerateMap()
	{
		float[,] noiseMap = PerlinNoise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale,
														octaves, persistance, lacunarity,
														seed, offset);

		Color[] colorMap = new Color[mapWidth * mapHeight];
		for(int i = 0; i < mapWidth * mapHeight; ++i)
		{
			int x = i % mapWidth;
			int y = i / mapWidth;
			foreach(TerrainType terrainType in regions)
			{
				if (noiseMap[x, y] <= terrainType.height)
				{
					colorMap[i] = terrainType.color;
					break;
				}
			}
		}

		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		}
		else if(drawMode == DrawMode.Colourmap) 
		{
			display.DrawTexture(TextureGenerator.TextureFromColourMap(colorMap, mapWidth, mapHeight));
		}
		else if(drawMode == DrawMode.Mesh)
		{
			display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, noiseHeightMultiplier),
				TextureGenerator.TextureFromColourMap(colorMap, mapWidth, mapHeight));
		}
	}

	void OnValidate()
	{
		if(mapWidth < 2)
		{
			mapWidth = 2;
		}
		if(mapHeight < 2)
		{
			mapHeight = 2;
		}
		if(lacunarity < 1)
		{
			lacunarity = 1;
		}
		if(octaves < 1)
		{
			octaves = 1;
		}
		if(noiseScale < 0)
		{
			noiseScale = 0;
		}
	}
}

[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color color;
}