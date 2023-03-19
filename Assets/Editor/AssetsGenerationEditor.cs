using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssetsGeneratorManager))]
public class AssetsGenerationEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		AssetsGeneratorManager assetGen = (AssetsGeneratorManager)target;
		if (Application.isPlaying && GUILayout.Button("Generate"))
		{
			assetGen.GenerateAssets();
		}
	}
}
