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

	MapData mapData;
	Vector3[] points;
	Rect mapRect;

	static MapGenerator instance;
	public static MapGenerator Instance { get { return instance; } }
	public int Width { get { return mapData.heightMap.GetLength(0); } }
	public int Height { get { return mapData.heightMap.GetLength(1); } }
	public Rect MapRect { get { return mapRect; } }

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
	public void DrawMapAtPosition(Vector3 pos)
	{
		GenerateMapData();
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.uniformScale, pos);
		points = meshData.vertices;

		Vector3 btmLeft = GetCoordinateOfNode(0, 0);
		Vector3 topRight = GetCoordinateOfNode(mapData.heightMap.GetLength(0) - 1, mapData.heightMap.GetLength(1) - 1);
		mapRect = new Rect(
			btmLeft.x,
			btmLeft.z,
			topRight.x - btmLeft.x,
			topRight.z - btmLeft.z);

		MapDisplay.DrawMesh(meshData, meshFilter, meshCollider);
	}

	void GenerateMapData()
	{
		terrainData.GenerateNoiseMap();

		float[,] heightMap = terrainData.noiseMap.Clone() as float[,];
		float minHeight = float.MaxValue;
		float maxHeight = float.MinValue;

		for(int i = 0; i < terrainData.mapSize * terrainData.mapSize; ++i)
		{
			int x = i % terrainData.mapSize;
			int y = i / terrainData.mapSize;
			if (terrainData.useFalloffMap)
			{
				heightMap[x, y] = heightMap[x, y] * terrainData.falloffMap[x, y];
			}
			if (terrainData.useCurveMultiplier)
			{
				heightMap[x, y] *= terrainData.meshHeightCurveMultiplier.Evaluate(heightMap[x, y]);
			}
			heightMap[x, y] *= terrainData.noiseHeightMultiplier;
			minHeight = Mathf.Min(heightMap[x, y], minHeight);
			maxHeight = Mathf.Max(heightMap[x, y], maxHeight);
		}
		textureData.UpdateMeshHeight(terrainMaterial,
			minHeight * terrainData.uniformScale,
			maxHeight * terrainData.uniformScale);
		mapData = new MapData(heightMap, minHeight, maxHeight);
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
		int index = x + y * mapData.heightMap.GetLength(0);
		return points[index];
	}

	public Vector3? GetPointAtCoordinates(Vector2 coordinate2D)
	{
		RaycastHit raycastHit;
		Vector3 rayOrigin = new Vector3(coordinate2D.x, mapData.maxHeight * terrainData.uniformScale + 1, coordinate2D.y);
		float maxDist = (mapData.maxHeight - mapData.minHeight) * terrainData.uniformScale + 2;
		if (Physics.Raycast(rayOrigin, Vector3.down, out raycastHit, maxDist, LayerMask.GetMask(new[] {"Terrain"})))
		{
			return raycastHit.point;
		}
		return null;
	}
}

public struct MapData
{
	public readonly float[,] heightMap;
	public readonly float minHeight;
	public readonly float maxHeight;

	public MapData(float[,] heightMap, float minHeight, float maxHeight)
	{
		this.heightMap = heightMap;
		this.minHeight = minHeight;
		this.maxHeight = maxHeight;
	}
}