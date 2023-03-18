using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapDisplay
{
	public static void DrawMesh(MeshData meshData, MeshFilter meshFilter, MeshCollider meshCollider)
	{
		Mesh mesh = meshData.CreateMesh();
		meshFilter.sharedMesh = mesh;
		meshCollider.sharedMesh = mesh;
	}
}
