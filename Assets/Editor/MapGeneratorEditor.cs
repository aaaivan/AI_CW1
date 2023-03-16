using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		MapGenerator mapGen = (MapGenerator)target;

		if(DrawDefaultInspector())
		{
			if (mapGen.meshFilter == null ||
				mapGen.meshRenderer == null ||
				mapGen.meshCollider == null)
			{
				return;
			}
			mapGen.DrawMap();
		}
		if (GUILayout.Button("Generate"))
		{
			if (mapGen.meshFilter == null ||
				mapGen.meshRenderer == null ||
				mapGen.meshCollider == null)
			{
				return;
			}
			mapGen.DrawMap();
		}
	}
}
