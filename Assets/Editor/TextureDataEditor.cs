using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextureData))]
public class TextureDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		TextureData data = (TextureData)target;
		if (DrawDefaultInspector())
		{
			data.NotifyValueUpdated();
		}
		if (GUILayout.Button("Generate Texture"))
		{
			data.NotifyValueUpdated();
		}
	}
}
