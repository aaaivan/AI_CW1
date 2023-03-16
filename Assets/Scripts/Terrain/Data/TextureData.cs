using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TextureData : ScriptableObject
{
	public Color baseColor;
	public Color[] baseColours;
	[Range(0f, 1f)] public float[] baseStartHeight;
	[Range(0f, 1f)] public float[] baseBlends;


	public event System.Action OnValuesUpdated;

	public void NotifyValueUpdated()
	{
		if (OnValuesUpdated != null)
		{
			OnValuesUpdated();
		}
	}

	public void UpdateMeshHeight(Material material, float minHeight, float maxHeight)
	{
		material.SetInt("baseColoursCount", baseColours.Length);
		material.SetColor("baseColor", baseColor);
		material.SetColorArray("baseColours", baseColours);
		material.SetFloatArray("baseStartHeights", baseStartHeight);
		material.SetFloatArray("baseBlends", baseBlends);
		material.SetFloat("minHeight", minHeight);
		material.SetFloat("maxHeight", maxHeight);
	}
}
