using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public enum DrawMode {NoiseMap, Colourmap, Mesh};
	[SerializeField]
	DrawMode drawMode = DrawMode.NoiseMap;
	[HideInInspector]
	public const int mapChunckSize = 241;
	[SerializeField]
	[Range(0, 1.0f)]
	float mapScale = 1.0f;
	[SerializeField]
	[Range(0, 6)]
	int levelOfDetail = 0;
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

	MapData GenerateMapData()
	{
		float[,] noiseMap = PerlinNoise.GenerateNoiseMap(mapChunckSize, mapChunckSize, noiseScale,
														octaves, persistance, lacunarity,
														seed, offset);

		Color[] colorMap = new Color[mapChunckSize * mapChunckSize];
		for(int i = 0; i < mapChunckSize * mapChunckSize; ++i)
		{
			int x = i % mapChunckSize;
			int y = i / mapChunckSize;
			foreach(TerrainType terrainType in regions)
			{
				if (noiseMap[x, y] <= terrainType.height)
				{
					colorMap[i] = terrainType.color;
					break;
				}
			}
		}

		return new MapData(noiseMap, colorMap);
	}

	/*
	 * ========================================================
	 * Editor methods
	 * ========================================================
	 */
	public void DrawMapInEditor()
	{
		MapData mapData = GenerateMapData();
		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
		}
		else if (drawMode == DrawMode.Colourmap)
		{
			display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colorMap, mapChunckSize, mapChunckSize));
		}
		else if (drawMode == DrawMode.Mesh)
		{
			display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, mapScale, meshHeightMultiplier, noiseHeightMultiplier, levelOfDetail),
				TextureGenerator.TextureFromColourMap(mapData.colorMap, mapChunckSize, mapChunckSize));
		}
	}

	void OnValidate()
	{
		if(levelOfDetail < 0)
		{
			levelOfDetail = 0;
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

public struct MapData
{
	public readonly float[,] heightMap;
	public readonly Color[] colorMap;

	public MapData(float[,] heightMap, Color[] colorMap)
	{
		this.heightMap = heightMap;
		this.colorMap = colorMap;
	}
}