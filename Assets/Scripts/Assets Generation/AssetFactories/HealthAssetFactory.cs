using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAssetFactory : MonoBehaviour , IAssetFactory
{
	public GameObject healthPrefab;
	[Range(0, 1.0f)]
	public float accessibleNodeProbability;

	public float GetProbabilityAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		AStarNode node = playerAgent.NodeFromWorldPos(pos);
		return node.accessible ? accessibleNodeProbability : 0;
	}

	public void SpawnAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		Instantiate(healthPrefab, pos, Quaternion.identity, terrain.transform);
	}
}
