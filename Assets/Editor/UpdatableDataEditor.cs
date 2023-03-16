using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseData))]
public class UpdatableDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		NoiseData data = (NoiseData)target;
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
