using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : CollectibleItem
{
	public override void Awake()
	{
		itemType = ItemType.Poison;
		base.Awake();
	}
}