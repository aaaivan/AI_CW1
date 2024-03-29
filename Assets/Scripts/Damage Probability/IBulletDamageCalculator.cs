using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletDamageCalculator
{
	public int GetBulletDamage();
	public void ActivateDamagePowerup(float powerupDuration);
}
