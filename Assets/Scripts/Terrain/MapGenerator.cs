using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public enum DrawMode {NoiseMap, Colourmap, FallOffMap};

	public DrawMode drawMode = DrawMode.NoiseMap;
	public TerrainData terrainData;
	public TextureData textureData;

	public TerrainType[] regions;

	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;
	public Material terrainMaterial;

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshCollider = GetComponent<MeshCollider>();

		drawMode = DrawMode.Colourmap;
		terrainData.seed = (int)DateTime.Now.Ticks;
		DrawMap();
	}

	MapData GenerateMapData()
	{
		float[,] noiseMap = terrainData.noiseMap.Clone() as float[,];
		float minHeight = float.MaxValue;
		float maxHeight = float.MinValue;

		Color[] colorMap = new Color[terrainData.mapSize * terrainData.mapSize];
		for(int i = 0; i < terrainData.mapSize * terrainData.mapSize; ++i)
		{
			int x = i % terrainData.mapSize;
			int y = i / terrainData.mapSize;
			if(terrainData.useFalloffMap)
			{
				noiseMap[x, y] = noiseMap[x, y] * terrainData.falloffMap[x, y];
			}
			if (terrainData.useCurveMultiplier)
			{
				noiseMap[x, y] *= terrainData.meshHeightCurveMultiplier.Evaluate(noiseMap[x, y]);
			}
			minHeight = Mathf.Min(noiseMap[x, y], minHeight);
			maxHeight = Mathf.Max(noiseMap[x, y], maxHeight);

			foreach (TerrainType terrainType in regions)
			{
				if (noiseMap[x, y] <= terrainType.height)
				{
					colorMap[i] = terrainType.color;
					break;
				}
			}
		}
		textureData.UpdateMeshHeight(terrainMaterial,
			minHeight * terrainData.mapScale * terrainData.noiseHeightMultiplier,
			maxHeight * terrainData.mapScale * terrainData.noiseHeightMultiplier);
		return new MapData(noiseMap, colorMap);
	}

	public void DrawMap()
	{
		if (drawMode == DrawMode.NoiseMap)
		{
			MapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(terrainData.noiseMap, terrainData.mapScale, terrainData.noiseHeightMultiplier),
				TextureGenerator.TextureFromHeightMap(terrainData.noiseMap), meshFilter, meshRenderer, meshCollider);
		}
		else if (drawMode == DrawMode.Colourmap)
		{
			MapData mapData = GenerateMapData();

			MapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.mapScale, terrainData.noiseHeightMultiplier),
				TextureGenerator.TextureFromColourMap(mapData.colorMap, terrainData.mapSize, terrainData.mapSize), meshFilter, meshRenderer, meshCollider);
		}
		else if(drawMode == DrawMode.FallOffMap)
		{
			MapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(terrainData.falloffMap, terrainData.mapScale, terrainData.noiseHeightMultiplier),
				TextureGenerator.TextureFromHeightMap(terrainData.falloffMap), meshFilter, meshRenderer, meshCollider);
		}
	}

	public void OnNoiseValuesUpdated()
	{
		if (meshFilter == null ||
			meshRenderer == null ||
			meshCollider == null ||
			terrainMaterial == null)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			DrawMap();
		}
	}

	void OnTextureValuesUpdated()
	{
		OnNoiseValuesUpdated();
	}

	void OnValidate()
	{
		if(terrainData != null)
		{
			terrainData.OnValuesUpdated -= OnNoiseValuesUpdated;
			terrainData.OnValuesUpdated += OnNoiseValuesUpdated;
		}
		if (textureData != null)
		{
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
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