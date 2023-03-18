using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsGeneratorTest : MonoBehaviour
{
	public AStarGrid grid;
	public float radius = 1;
	public int rejectionSamples = 30;
	public int itemsCount = 100;

	List<Vector2> points;

	void OnValidate()
	{
		if(grid != null)
		{
			points = PoissonDiscSampling.GenerateDistribution(radius, grid.mapGenerator.mapRect, grid, itemsCount, rejectionSamples);
		}
	}

	void OnDrawGizmos()
	{
		if (points != null)
		{
			foreach (Vector2 point in points)
			{
				Vector3 pos = new Vector3(
					point.x + grid.mapGenerator.mapRect.x,
					10,
					point.y + grid.mapGenerator.mapRect.y);
				Gizmos.DrawSphere(pos, radius/2);
			}
		}
	}
}
