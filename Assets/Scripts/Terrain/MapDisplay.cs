using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapDisplay
{
	public static void DrawMesh(MeshData meshData, Texture2D texture, MeshFilter meshFilter,
		MeshRenderer meshRenderer, MeshCollider meshCollider)
	{
		Mesh mesh = meshData.CreateMesh();
		meshFilter.sharedMesh = mesh;
		meshCollider.sharedMesh = mesh;
	}
}
