using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : CollectibleItem
{
	public override void Awake()
	{
		itemType = ItemType.Health;
		base.Awake();
	}
}