using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PathfinderData : ScriptableObject
{
	public LayerMask unwalkableLayers;
	[Range(0, 10)] public float maxWalkableHeight;
	[Range(0.0f, 2.0f)] public float maxWalkableSlope;

	public event System.Action OnValuesUpdated;

	public void NotifyValueUpdated()
	{
		if (OnValuesUpdated != null)
		{
			OnValuesUpdated();
		}
	}

	private void OnValidate()
	{
		if(maxWalkableSlope < 0)
		{
			maxWalkableSlope = 0;
		}
		else if (maxWalkableSlope > 90)
		{
			maxWalkableSlope = 90;
		}
		NotifyValueUpdated();
	}
}
