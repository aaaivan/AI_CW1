using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiscSampling
{
	public static List<Vector2> GenerateDistribution(Vector2 sampleRegion, float radius, int numSamplesBeforeRejection, Vector2? seed = null)
	{
		float cellSize = radius / Mathf.Sqrt(2);
		int width = Mathf.CeilToInt(sampleRegion.x/cellSize);
		int height = Mathf.CeilToInt(sampleRegion.y /cellSize);
		int itemCount = 0;

		int[,] grid = new int[width, height];
		List<Vector2> points = new List<Vector2>();
		List<Vector2> spawnPoints = new List<Vector2>();
		
		if(seed != null)
		{
			Vector2 point = new Vector2(
				Mathf.Clamp(seed.Value.x, 0, sampleRegion.x),
				Mathf.Clamp(seed.Value.y, 0, sampleRegion.y));
			spawnPoints.Add(point);
		}
		else
		{
			spawnPoints.Add(new Vector2(Random.value * sampleRegion.x,
							Random.value * sampleRegion.y));
		}

		while (spawnPoints.Count > 0)
		{
			int spawnIndex = Random.Range(0, spawnPoints.Count);
			Vector2 spawnCenter = spawnPoints[spawnIndex];

			bool success = false;
			for(int i = 0; i < numSamplesBeforeRejection; i++)
			{
				float angle = Random.value * Mathf.PI * 2;
				Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 item = spawnCenter + dir * Random.Range(radius, 2 * radius);

				if(IsValid(item, sampleRegion, cellSize, radius, points, grid))
				{
					points.Add(item);
					spawnPoints.Add(item);
					grid[(int)(item.x / cellSize), (int)(item.y / cellSize)] = points.Count;
					success = true;
					++itemCount;
					break;
				}
			}
			if(!success)
			{
				spawnPoints.RemoveAt(spawnIndex);
			}
		}

		return points;
	}

	static bool IsValid(Vector2 item, Vector2 sampleRegion, float cellSize, float radius, List<Vector2> points, int[,] grid)
	{
		if(item.x > 0 && item.x < sampleRegion.x &&
			item.y > 0 && item.y < sampleRegion.y)
		{
			int cellX = (int)(item.x / cellSize);
			int cellY = (int)(item.y / cellSize);
			int startX = Mathf.Max(0, cellX - 2);
			int endX = Mathf.Min(grid.GetLength(0) - 1, cellX + 2);
			int startY = Mathf.Max(0, cellY - 2);
			int endY = Mathf.Min(grid.GetLength(1) - 1, cellY + 2);

			for (int y = startY; y <= endY; y++)
			{
				for (int x = startX; x <= endX; x++)
				{
					int pointIndex = grid[x,y] - 1;
					if(pointIndex >= 0)
					{
						float dist = (item - points[pointIndex]).sqrMagnitude;
						if(dist < radius * radius)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		return false;
	}
}
