using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamageCalculator_Player : MonoBehaviour, IBulletDamageCalculator
{
	int numberOfTosses = 3;
	int maxValueOfEachToss = 8;
	int criticalHitDamage = 25;
	float criticalHitProbability = 0.1f;
	float criticalHitProbabilityWithPowerup = 0.2f;

	float powerupBonusDamage = 0.5f;
	float timePowerupWasActivated = 0;
	float powerupDuration = 0;

	public void ActivateDamagePowerup(float duration)
	{
		timePowerupWasActivated = Time.time;
		powerupDuration = duration;
	}

	// symmetrical damage profile with critical hit
	public int GetBulletDamage()
	{
		bool powerupActive = Time.time < timePowerupWasActivated + powerupDuration;
		float _criticalHitProb = powerupActive ? criticalHitProbabilityWithPowerup : criticalHitProbability;

		int damage = 0;
		if(Random.value < _criticalHitProb)
		{
			damage = criticalHitDamage;
		}
		else
		{
			damage = DiceToss.TossDice(numberOfTosses, maxValueOfEachToss);
		}

		if(powerupActive)
		{
			damage += Mathf.RoundToInt(damage * powerupBonusDamage);
		}

		return damage;
	}
}
