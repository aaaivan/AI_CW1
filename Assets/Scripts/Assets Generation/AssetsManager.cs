using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AssetsManager : MonoBehaviour
{
	public int numberOfCentroids = 2;
	public bool drawClusters = false;

	List<Transform> items = new List<Transform>();
	List<Vector3> positions = new List<Vector3>();
	KMeansData clusterData = new KMeansData();

	static AssetsManager instance;
	public static AssetsManager Instance { get { return instance; } }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	public void AddItem(GameObject item)
	{
		items.Add(item.transform);
		positions.Add(item.transform.position);
	}

	public void ClearItems()
	{
		foreach (Transform item in items)
		{
			Destroy(item.gameObject);
		}
		items.Clear();
		positions.Clear();
		clusterData = new KMeansData();
	}

	public void CalculateClusters()
	{
		clusterData = KMeans.FindClusters(positions, numberOfCentroids);
	}

	private void OnDrawGizmos()
	{
		if(clusterData.clusters != null && drawClusters)
		{
			for (int i = 0; i < clusterData.clusters.Length; i++)
			{
				int cluster = clusterData.clusters[i];
				Gizmos.color = GetColorForCluster(cluster);
				Gizmos.DrawSphere(positions[i], 1);
				Gizmos.DrawLine(clusterData.centroids[cluster], positions[i]);
			}
		}
	}

	Color GetColorForCluster(int i)
	{
		i %= 8;
		switch (i)
		{
		case 0:
			return Color.red;
		case 1:
			return Color.green;
		case 2:
			return Color.blue;
		case 3:
			return Color.yellow;
		case 4:
			return Color.magenta;
		case 5:
			return Color.cyan;
		case 6:
			return Color.white;
		case 7:
			return Color.black;
		}
		return Color.white;
	}

	private void OnValidate()
	{
		if(numberOfCentroids < 1)
		{
			numberOfCentroids = 1;
		}
		else if(numberOfCentroids > 8)
		{
			numberOfCentroids = 8;
		}
		if(clusterData.centroids != null &&
			numberOfCentroids != clusterData.centroids.Length)
		{
			CalculateClusters();
		}
	}
}
