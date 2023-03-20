using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : CollectibleItem
{
	public override void Awake()
	{
		itemType = ItemType.Diamond;
		base.Awake();
	}
}