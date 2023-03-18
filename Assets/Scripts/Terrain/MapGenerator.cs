using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public TerrainData terrainData;
	public TextureData textureData;

	public MeshFilter meshFilter;
	public MeshCollider meshCollider;
	public Material terrainMaterial;

	[HideInInspector] public float[,] heightMap;
	[HideInInspector] public Vector3[] points;
	[HideInInspector] public Rect mapRect;

	static MapGenerator instance;
	public static MapGenerator Instance { get { return instance; } }

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			meshFilter = GetComponent<MeshFilter>();
			meshCollider = GetComponent<MeshCollider>();
			terrainData.seed = (int)DateTime.Now.Ticks;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		if(instance == this)
		{
			instance = null;
		}
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
			MapData mapData = GenerateMapData();
			MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.uniformScale, terrainData.noiseHeightMultiplier, pos);
			points = meshData.vertices;

			Vector3 btmLeft = GetCoordinateOfNode(0, 0);
			Vector3 topRight = GetCoordinateOfNode(heightMap.GetLength(0) - 1, heightMap.GetLength(1) - 1);
			mapRect = new Rect(
				btmLeft.x,
				btmLeft.z,
				topRight.x - btmLeft.x,
				topRight.z - btmLeft.z);

			MapDisplay.DrawMesh(meshData, meshFilter, meshCollider);
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