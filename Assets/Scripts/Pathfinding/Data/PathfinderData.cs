using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PathfinderData : ScriptableObject
{
	public LayerMask unwalkableLayers;
	[Range(0, 50)] public int maxWalkableHeight;
	[Range(0.0f, 90.0f)] public float maxWalkableSlope;
	[Range(0.0f, 50.0f)] public float slopeAvoidanceFactor;

	public float GetCostForSlope(float angle)
	{
		angle = Mathf.Abs(angle);

		if(angle >= maxWalkableSlope)
			return float.MaxValue;

		return (slopeAvoidanceFactor * angle) / (maxWalkableSlope - angle);
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
		if(slopeAvoidanceFactor < 0)
		{
			slopeAvoidanceFactor = 0;
		}
	}
}
