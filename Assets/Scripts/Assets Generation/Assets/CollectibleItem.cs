using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
	public enum ItemType
	{
		Health,
		Diamond,
		Tree,
		Powerup,
		Coin,
		Poison,

		MAX_ITEM_TYPES
	}
	[HideInInspector] public string itemName;
	[HideInInspector] public ItemType itemType;

	public virtual void Awake()
	{
		itemName = ItemNameFromEnum(itemType);
		AssetsManager.Instance.AddItem(this);
	}

	public void OnObjectCollected()
	{
		Debug.Log(itemName);
	}

	public static string ItemNameFromEnum(ItemType item)
	{
		switch (item)
		{
			case ItemType.Health:
				return "Health";
			case ItemType.Diamond:
				return "Diamond";
			case ItemType.Tree:
				return "Tree";
			case ItemType.Powerup:
				return "Powerup";
			case ItemType.Coin:
				return "Coin";
			case ItemType.Poison:
				return "Poison";
		}
		return "";
	}
}
