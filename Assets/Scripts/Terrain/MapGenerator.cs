using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public enum DrawMode {NoiseMap, Colourmap, FallOffMap};

	public DrawMode drawMode = DrawMode.NoiseMap;
	public NoiseData noiseData;

	public TerrainType[] regions;

	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshCollider = GetComponent<MeshCollider>();

		drawMode = DrawMode.Colourmap;
		DrawMap();
	}

	MapData GenerateMapData()
	{
		float[,] noiseMap = noiseData.noiseMap.Clone() as float[,];

		Color[] colorMap = new Color[noiseData.mapSize * noiseData.mapSize];
		for(int i = 0; i < noiseData.mapSize * noiseData.mapSize; ++i)
		{
			int x = i % noiseData.mapSize;
			int y = i / noiseData.mapSize;
			if(noiseData.useFalloffMap)
			{
				noiseMap[x, y] = noiseMap[x, y] * noiseData.falloffMap[x, y];
			}
			if (noiseData.useCurveMultiplier)
			{
				noiseMap[x, y] *= noiseData.meshHeightMultiplier.Evaluate(noiseMap[x, y]);
			}
			foreach (TerrainType terrainType in regions)
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

	public void DrawMap()
	{
		if (drawMode == DrawMode.NoiseMap)
		{
			MapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseData.noiseMap, noiseData.mapScale, noiseData.noiseHeightMultiplier),
				TextureGenerator.TextureFromHeightMap(noiseData.noiseMap), meshFilter, meshRenderer, meshCollider);
		}
		else if (drawMode == DrawMode.Colourmap)
		{
			MapData mapData = GenerateMapData();

			MapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, noiseData.mapScale, noiseData.noiseHeightMultiplier),
				TextureGenerator.TextureFromColourMap(mapData.colorMap, noiseData.mapSize, noiseData.mapSize), meshFilter, meshRenderer, meshCollider);
		}
		else if(drawMode == DrawMode.FallOffMap)
		{
			MapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseData.falloffMap, noiseData.mapScale, noiseData.noiseHeightMultiplier),
				TextureGenerator.TextureFromHeightMap(noiseData.falloffMap), meshFilter, meshRenderer, meshCollider);
		}
	}

	void OnValidate()
	{
		if(noiseData != null && !Application.isPlaying)
		{
			noiseData.OnValuesUpdated -= DrawMap;
			noiseData.OnValuesUpdated += DrawMap;
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