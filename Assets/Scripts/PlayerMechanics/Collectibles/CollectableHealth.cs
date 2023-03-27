using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableHealth : CollectableItem
{
	int healAmount = 100;
	protected override void OnObjectCollected(Transform collectedBy)
	{
		collectedBy.GetComponent<DamageableObject>().Heal(healAmount);
		base.OnObjectCollected(collectedBy);
	}
}
