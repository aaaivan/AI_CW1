using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsFactory : MonoBehaviour, IAssetFactory
{
	public GameObject coinPrefab;
	[Range(0, 1.0f)]
	public float accessibleNodeProbability;
	public float clusterSize;
	public float minDistanceBetweenItems;
	public int iterationsBeforeRejection = 50;

	public float GetProbabilityAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		AStarNode node = playerAgent.NodeFromWorldPos(pos);
		return node.accessible ? accessibleNodeProbability : 0;
	}

	public void SpawnAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		List<Vector2> points = PoissonDiscSampling.GenerateDistribution(new Vector2(clusterSize, clusterSize), minDistanceBetweenItems, iterationsBeforeRejection);

		foreach (var point in points)
		{
			Vector3? coinPos = terrain.GetPointAtCoordinates(new Vector2(pos.x, pos.z) + point - new Vector2(clusterSize / 2, clusterSize / 2));
			if (pos == null) continue;
			if (playerAgent.NodeFromWorldPos(coinPos.Value).accessible)
			{
				GameObject go = Instantiate(coinPrefab, coinPos.Value, Quaternion.identity, terrain.transform);
				AssetsManager.Instance.AddItem(go);
			}
		}
	}
}
