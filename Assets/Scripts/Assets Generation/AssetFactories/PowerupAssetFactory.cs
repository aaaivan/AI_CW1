using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupAssetFactory : MonoBehaviour, IAssetFactory
{
	public GameObject powerupPrefab;
	[Range(0, 1.0f)]
	public float accessibleNodeProbability;
	public AnimationCurve yAxesDistribution;

	public float GetProbabilityAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		AStarNode node = playerAgent.NodeFromWorldPos(pos);
		return yAxesDistribution.Evaluate((pos.z - terrain.MapRect.y) / terrain.MapRect.height) * (node.accessible ? accessibleNodeProbability : 0);
	}

	public void SpawnAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		Instantiate(powerupPrefab, pos, Quaternion.identity, terrain.transform);
	}
}
