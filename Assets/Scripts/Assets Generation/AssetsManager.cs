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

	List<CollectibleItem> items = new List<CollectibleItem>();
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

	public void AddItem(CollectibleItem item)
	{
		items.Add(item);
		positions.Add(item.transform.position);
	}

	public void ClearItems()
	{
		foreach (CollectibleItem item in items)
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
		CountClusters();
	}

	public void CountClusters()
	{
		int[,] itemCountsInClusters = new int[clusterData.centroids.Length, (int)CollectibleItem.ItemType.MAX_ITEM_TYPES];
		for( int i = 0; i < items.Count; i++)
		{
			CollectibleItem collectableItem = items[i];
			if(collectableItem != null)
			{
				itemCountsInClusters[clusterData.clusters[i], (int)collectableItem.itemType] += 1;
			}
		}

		string message = "";
		for (int centroid = 0; centroid < itemCountsInClusters.GetLength(0); centroid++)
		{
			message += "\nCluster " + centroid.ToString() + ": ";
			for(int itemType = 0;  itemType < itemCountsInClusters.GetLength(1); itemType++)
			{
				int count = itemCountsInClusters[centroid, itemType];
				string s = string.Format("{0} ({1}) - ", CollectibleItem.ItemNameFromEnum((CollectibleItem.ItemType)itemType), count.ToString());
				message += s;
			}
		}
		Debug.Log(message);
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
