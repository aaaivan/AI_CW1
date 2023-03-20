using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : CollectibleItem
{
	public override void Awake()
	{
		itemType = ItemType.Coin;
		base.Awake();
	}
}