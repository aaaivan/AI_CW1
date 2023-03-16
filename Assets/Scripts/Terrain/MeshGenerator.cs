using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float scaleFactor, float heightMultiplier)
	{
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);

		Vector2 offset = new Vector2((1 - width)/2.0f, (1 - height)/2.0f);

		MeshData meshData = new MeshData(width, height);
		int vertexIndex = 0;
		int uvsIndex = 0;
		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				float xCoord = (x + offset.x) * scaleFactor;
				float yCoord = heightMap[x, y] * heightMultiplier * scaleFactor;
				float zCoord = (y + offset.y) * scaleFactor;
				meshData.AddVertex(ref vertexIndex, new Vector3(xCoord, yCoord, zCoord));
				meshData.AddUV(ref uvsIndex, new Vector2((float)x/width, (float)y/height));
			}
		}

		int trisIndex = 0;
		for (int y = 0; y < height - 1; y++)
		{
			for (int x = 0; x < width - 1; x++)
			{
				int a0 = y * width + x;
				int a1 = a0 + width;
				int a2 = a1 + 1;
				int a3 = a0 + 1;
				meshData.AddTriangle(ref trisIndex, a0, a1, a3);
				meshData.AddTriangle(ref trisIndex, a2, a3, a1);
			}
		}

		return meshData;
	}
}

public class MeshData
{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;


	public MeshData(int meshWidth,  int meshHeight)
	{
		vertices = new Vector3[meshWidth * meshHeight];
		uvs = new Vector2[meshWidth * meshHeight];
		triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
	}

	public void AddVertex(ref int index, Vector3 point)
	{
		vertices[index++] = point;
	}

	public void AddUV(ref int index, Vector2 point)
	{
		uvs[index++] = point;
	}

	public void AddTriangle(ref int index, int v1, int v2, int v3)
	{
		triangles[index++] = v1;
		triangles[index++] = v2;
		triangles[index++] = v3;
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		return mesh;
	}
}