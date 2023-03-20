using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAssetFactory : MonoBehaviour, IAssetFactory
{
	public GameObject healthPrefab;
	[Range(0, 1.0f)]
	public float accessibleNodeProbability;
	[Range(0, 1.0f)]
	public float unaccessibleNodeProbability;

	public float GetProbabilityAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		AStarNode node = playerAgent.NodeFromWorldPos(pos);
		return node.accessible ? accessibleNodeProbability : unaccessibleNodeProbability;
	}

	public void SpawnAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		GameObject go = Instantiate(healthPrefab, pos, Quaternion.identity, terrain.transform);
		AssetsManager.Instance.AddItem(go);
	}
}
