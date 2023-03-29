using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class AssetsManager : MonoBehaviour
{
	public int numberOfCentroids = 2;
	public bool drawClusters = false;
	public CollectableItemType highlightItem = CollectableItemType.MAX_ITEM_TYPES;

	[HideInInspector] public List<CollectableItem> collectableItems = new List<CollectableItem>();
	[HideInInspector] public List<Vector3> positions = new List<Vector3>();
	[HideInInspector] public Dictionary<CollectableItemType, List<int>> itemsByType = new Dictionary<CollectableItemType, List<int>>();

	KMeansData clusterData = new KMeansData();

	static AssetsManager instance;
	public static AssetsManager Instance { get { return instance; } }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

			for(int i = 0; i < (int)CollectableItemType.MAX_ITEM_TYPES; ++i)
			{
				itemsByType.Add((CollectableItemType)i, new List<int>());
			}
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

	public void AddItem(CollectableItem item)
	{
		itemsByType[item.itemType].Add(collectableItems.Count);
		collectableItems.Add(item);
		positions.Add(item.transform.position);
	}

	public void ClearItems()
	{
		foreach (CollectableItem item in collectableItems)
		{
			if(item != null)
			{
				Destroy(item.gameObject);
			}
		}
		collectableItems.Clear();
		positions.Clear();
		foreach(var listOfType in itemsByType)
		{
			listOfType.Value.Clear();
		}
		clusterData = new KMeansData();
	}

	public void CalculateClusters()
	{
		clusterData = KMeans.FindClusters(positions, numberOfCentroids);
		CountClusters();
	}

	public void CountClusters()
	{
		int[] itemCountsPerType = new int[(int)CollectableItemType.MAX_ITEM_TYPES];
		int[,] itemCountsInClusters = new int[clusterData.centroids.Length, (int)CollectableItemType.MAX_ITEM_TYPES];
		for ( int i = 0; i < collectableItems.Count; i++)
		{
			CollectableItem collectableItem = collectableItems[i];
			if(collectableItem != null)
			{
				itemCountsInClusters[clusterData.clusters[i], (int)collectableItem.itemType] += 1;
				itemCountsPerType[(int)collectableItem.itemType] += 1;
			}
		}

		string message = "\nTotal counts: ";
		for (int itemType = 0; itemType < itemCountsPerType.Length; itemType++)
		{
			string itemName = CollectableItemData.ItemNameFromEnum((CollectableItemType)itemType);
			int count = itemCountsPerType[itemType];
			string s = string.Format("{0} ({1}) - ", itemName, count.ToString());
			message += s;
		}
		for (int centroid = 0; centroid < itemCountsInClusters.GetLength(0); centroid++)
		{
			message += "\nCluster " + centroid.ToString() + ": ";
			for(int itemType = 0;  itemType < itemCountsInClusters.GetLength(1); itemType++)
			{
				string itemName = CollectableItemData.ItemNameFromEnum((CollectableItemType)itemType);
				int count = itemCountsInClusters[centroid, itemType];
				string s = string.Format("{0} ({1}) - ", itemName, count.ToString());
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
				Gizmos.DrawSphere(positions[i], MapGenerator.Instance.terrainData.uniformScale/2);
				Gizmos.DrawLine(clusterData.centroids[cluster], positions[i]);
			}
		}
		else if(highlightItem != CollectableItemType.MAX_ITEM_TYPES)
		{
			foreach(int i in itemsByType[highlightItem])
			{
				Gizmos.color = Color.black;
				Gizmos.DrawSphere(positions[i], MapGenerator.Instance.terrainData.uniformScale);
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
