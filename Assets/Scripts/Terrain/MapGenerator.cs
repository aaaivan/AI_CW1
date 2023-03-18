using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public enum DrawMode {NoiseMap, Mesh, FallOffMap};

	public DrawMode drawMode = DrawMode.NoiseMap;
	public TerrainData terrainData;
	public TextureData textureData;

	public MeshFilter meshFilter;
	public MeshCollider meshCollider;
	public Material terrainMaterial;

	[HideInInspector] public float[,] heightMap;
	[HideInInspector] public Vector3[] points;

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshCollider = GetComponent<MeshCollider>();

		drawMode = DrawMode.Mesh;
		terrainData.seed = (int)DateTime.Now.Ticks;
		DrawMapAtPosition(transform.position);
	}

	#region MAP_GENERATION
	MapData GenerateMapData()
	{
		terrainData.GenerateNoiseMap();

		heightMap = terrainData.noiseMap.Clone() as float[,];
		float minHeight = float.MaxValue;
		float maxHeight = float.MinValue;

		for(int i = 0; i < terrainData.mapSize * terrainData.mapSize; ++i)
		{
			int x = i % terrainData.mapSize;
			int y = i / terrainData.mapSize;
			if(terrainData.useFalloffMap)
			{
				heightMap[x, y] = heightMap[x, y] * terrainData.falloffMap[x, y];
			}
			if (terrainData.useCurveMultiplier)
			{
				heightMap[x, y] *= terrainData.meshHeightCurveMultiplier.Evaluate(heightMap[x, y]);
			}
			minHeight = Mathf.Min(heightMap[x, y], minHeight);
			maxHeight = Mathf.Max(heightMap[x, y], maxHeight);
		}
		textureData.UpdateMeshHeight(terrainMaterial,
			minHeight * terrainData.uniformScale * terrainData.noiseHeightMultiplier,
			maxHeight * terrainData.uniformScale * terrainData.noiseHeightMultiplier);
		return new MapData(heightMap);
	}

	public void DrawMapAtPosition(Vector3 pos)
	{
		if (drawMode == DrawMode.NoiseMap)
		{
			MapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(terrainData.noiseMap, terrainData.uniformScale, terrainData.noiseHeightMultiplier, transform.position), meshFilter, meshCollider);
		}
		else if (drawMode == DrawMode.Mesh)
		{
			MapData mapData = GenerateMapData();
			MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.uniformScale, terrainData.noiseHeightMultiplier, pos);
			points = meshData.vertices;
			MapDisplay.DrawMesh(meshData, meshFilter, meshCollider);
		}
		else if(drawMode == DrawMode.FallOffMap)
		{
			MapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(terrainData.falloffMap, terrainData.uniformScale, terrainData.noiseHeightMultiplier, transform.position), meshFilter, meshCollider);
		}
	}

	public void OnNoiseValuesUpdated()
	{
		if (meshFilter == null ||
			meshCollider == null ||
			terrainMaterial == null)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			DrawMapAtPosition(Vector3.zero);
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
	#endregion

	public Vector3 GetCoordinateOfNode(int x, int y)
	{
		int index = x + y * heightMap.GetLength(0);
		return points[index];
	}

}

public struct MapData
{
	public readonly float[,] heightMap;

	public MapData(float[,] heightMap)
	{
		this.heightMap = heightMap;
	}
}