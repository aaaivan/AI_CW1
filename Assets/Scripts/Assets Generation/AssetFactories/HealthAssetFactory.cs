using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAssetFactory : MonoBehaviour , IAssetFactory
{
	public GameObject healthPrefab;

	public float GetProbabilityAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		AStarNode node = playerAgent.NodeFromWorldPos(pos);
		return node.walkable ? 1 : 0;
	}

	public void SpawnAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		Instantiate(healthPrefab, pos, Quaternion.identity, terrain.transform);
	}
}
