using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public static class KMeans
{
	public static KMeansData FindClusters(List<Vector3> points, int numberOfCentroids)
	{
		numberOfCentroids = Mathf.Clamp(numberOfCentroids, 0, points.Count);
		int[] clusters = new int[points.Count];
		Vector3[] centroids = new Vector3[numberOfCentroids];

		for(int i = 0; i < numberOfCentroids; ++i)
		{
			centroids[i] = points[i];
		}

		while(true)
		{
			bool done = true;
			Vector3[] newCentroinds = new Vector3[numberOfCentroids];
			int[] itemsInEachCluster = new int[numberOfCentroids];
			for (int i = 0; i < points.Count; ++i)
			{
				int c = FindClosestCentroid(points[i], centroids);
				if (clusters[i] != c)
				{
					done = false;
				}
				clusters[i] = c;
				newCentroinds[c] += points[i];
				itemsInEachCluster[c] += 1;
			}

			for(int i = 0; i < newCentroinds.Length; ++i)
			{
				centroids[i] = newCentroinds[i] / itemsInEachCluster[i];
			}


			if (done) break;
		}

		return new KMeansData(clusters, centroids);
	}

	static int FindClosestCentroid(Vector3 point, Vector3[] centroids)
	{
		float minDistSquared = float.MaxValue;
		int closestCentroidIndex = 0;
		for (int i = 0; i < centroids.Length; ++i)
		{
			float distSquared = (centroids[i] - point).sqrMagnitude;
			if (distSquared < minDistSquared)
			{
				minDistSquared = distSquared;
				closestCentroidIndex = i;
			}
		}
		return closestCentroidIndex;
	}
}

public struct KMeansData
{
	public readonly int[] clusters;
	public readonly Vector3[] centroids;
	public KMeansData(int[] clusters, Vector3[] centroids)
	{
		this.clusters = clusters;
		this.centroids = centroids;
	}
}