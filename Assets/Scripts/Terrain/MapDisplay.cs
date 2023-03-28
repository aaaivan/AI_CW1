using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapDisplay
{
	// creates a mesh and a mesh collider starting from a collection of points, tris, and uvs values
	public static void DrawMesh(MeshData meshData, MeshFilter meshFilter, MeshCollider meshCollider)
	{
		Mesh mesh = meshData.CreateMesh();
		meshFilter.sharedMesh = mesh;
		meshCollider.sharedMesh = mesh;
	}
}
