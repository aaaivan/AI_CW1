using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetFactory : MonoBehaviour
{
	public CollectableItemData itemData;
	public void SpawnAtLocation(Vector3 pos, PathfinderAgent playerAgent, MapGenerator terrain)
	{
		GameObject go = Instantiate(itemData.itemPrefab, pos, Quaternion.identity, terrain.transform);
		go.GetComponent<CollectableItem>().Init(itemData.type, itemData.spins);
	}

	public float GetProbabilityAtLocation(Vector3 pos, PathfinderAgent playerAgent, MapGenerator terrain)
	{
		float radius = Mathf.Max(itemData.Collider.bounds.extents.x, itemData.Collider.bounds.extents.z);
		bool collidesCharacter = Physics.CheckSphere(pos, radius, LayerMask.NameToLayer("Character"));
		if (collidesCharacter) { return 0; }

		PathfinderNode node = playerAgent.NodeFromWorldPos(pos);
		if ((node.accessible && !itemData.canGoOnAcessibleNodes) ||
			(!node.accessible && !itemData.canGoOnUnacessibleNodes)) { return 0; }

		float probabilityDampingFactor = 1f;
		if(itemData.proximityPenalty > 0)
		{
			foreach (var i in AssetsManager.Instance.itemsByType[itemData.type])
			{
				float distance = (pos - AssetsManager.Instance.positions[i]).magnitude;
				if(distance < AssetsGeneratorManager.Instance.minDistanceBetweenItems * 2)
				{
					probabilityDampingFactor = 0;
					break;
				}
				float repulsionFactor = itemData.proximityPenalty + 1;
				float damping = Mathf.Clamp01(Mathf.InverseLerp(0, AssetsGeneratorManager.Instance.minDistanceBetweenItems * repulsionFactor, distance));
				probabilityDampingFactor *= damping;
			}
		}

		return probabilityDampingFactor;
	}
}
