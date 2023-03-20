using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : CollectibleItem
{
	public override void Awake()
	{
		itemType = ItemType.Powerup;
		base.Awake();
	}
}
