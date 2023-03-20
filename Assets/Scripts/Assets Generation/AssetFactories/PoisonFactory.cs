using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonFactory : MonoBehaviour, IAssetFactory
{
	public GameObject poisonPrefab;
	[Range(0, 1.0f)]
	public float accessibleNodeProbability;

	public float GetProbabilityAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		AStarNode node = playerAgent.NodeFromWorldPos(pos);
		return node.accessible ? accessibleNodeProbability : 0;
	}

	public void SpawnAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		GameObject go = Instantiate(poisonPrefab, pos, Quaternion.identity, terrain.transform);
		AssetsManager.Instance.AddItem(go);
	}
}
