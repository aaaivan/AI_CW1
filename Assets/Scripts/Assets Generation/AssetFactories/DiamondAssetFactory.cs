using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondAssetFactory : MonoBehaviour, IAssetFactory
{
	public GameObject healthPrefab;
	[Range(0, 1.0f)]
	public float accessibleNodeProbability;

	int id;
	public int ID { get { return id; } set { id = value; } }

	public float GetProbabilityAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		AStarNode node = playerAgent.NodeFromWorldPos(pos);
		return node.accessible ? accessibleNodeProbability : 0;
	}

	public void SpawnAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain)
	{
		GameObject go = Instantiate(healthPrefab, pos, Quaternion.identity, terrain.transform);
		AssetsManager.Instance.AddItem(go);
	}
}
