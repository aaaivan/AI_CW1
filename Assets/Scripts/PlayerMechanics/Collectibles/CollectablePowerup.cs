using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePowerup : CollectableItem
{
	float powerupDuration = 10.0f;
	protected override void OnObjectCollected(Transform collctedBy)
	{
		collctedBy.GetComponent<IBulletDamageCalculator>().ActivateDamagePowerup(powerupDuration);
		base.OnObjectCollected(collctedBy);
	}
}
