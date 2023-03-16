using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainData))]
public class TerrainDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		TerrainData data = (TerrainData)target;
		if (DrawDefaultInspector())
		{
			data.NotifyValueUpdated();
		}
		if (GUILayout.Button("Generate Map"))
		{
			data.NotifyValueUpdated();
		}
	}
}
