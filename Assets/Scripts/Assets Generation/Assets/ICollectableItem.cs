using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectableItem
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

	public ItemType Type { get; }
	public Vector3 Position { get; }
	public void OnObjectCollected();
}