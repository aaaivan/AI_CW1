using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : CollectibleItem
{
	public override void Awake()
	{
		itemType = ItemType.Tree;
		base.Awake();
	}
}
