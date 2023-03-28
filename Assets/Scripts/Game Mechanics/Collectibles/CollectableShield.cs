using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableShield : CollectableItem
{
	float powerupDuration = 10f;
    protected override void OnObjectCollected(Transform collectedBy)
	{
		collectedBy.GetComponent<IAttackMitigationCalculator>().ActivateShieldPowerup(powerupDuration);
		base.OnObjectCollected(collectedBy);
	}
}
