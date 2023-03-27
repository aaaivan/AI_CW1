using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableDiamond : CollectableItem
{
	int worth = 10;
	protected override void OnObjectCollected(Transform collctedBy)
	{
		collctedBy.GetComponent<PlayerScore>().AddScore(worth);
		base.OnObjectCollected(collctedBy);
	}
}
