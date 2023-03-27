using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCoin : CollectableItem
{
	int worth = 5;
	protected override void OnObjectCollected(Transform collctedBy)
	{
		collctedBy.GetComponent<PlayerScore>().AddScore(worth);
		base.OnObjectCollected(collctedBy);
	}
}
