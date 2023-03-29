using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CollectableItemData : ScriptableObject
{
	public CollectableItemType type;
	public GameObject itemPrefab;
	public bool canGoOnAcessibleNodes;
	public bool canGoOnUnacessibleNodes;
	public float proximityPenalty;

	public bool spins;

	public Collider Collider { get { return itemPrefab.GetComponent<Collider>(); } }
	public static string ItemNameFromEnum(CollectableItemType item)
	{
		switch (item)
		{
			case CollectableItemType.Health:
				return "Health";
			case CollectableItemType.Diamond:
				return "Diamond";
			case CollectableItemType.Tree:
				return "Tree";
			case CollectableItemType.Powerup:
				return "Powerup";
			case CollectableItemType.Coin:
				return "Coin";
			case CollectableItemType.Shield:
				return "Shield";
		}
		return "";
	}

	private void OnValidate()
	{
		if(proximityPenalty < 0)
		{
			proximityPenalty = 0;
		}
	}
}
public enum CollectableItemType
{
	Health,
	Diamond,
	Tree,
	Powerup,
	Coin,
	Shield,

	MAX_ITEM_TYPES
}
