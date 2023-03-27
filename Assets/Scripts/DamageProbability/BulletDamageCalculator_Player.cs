using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamageCalculator_Player : MonoBehaviour, IBulletDamageCalculator
{
	int numberOfTosses = 3;
	int maxValueOfEachToss = 8;
	int criticalHitDamage = 25;
	float criticalHitProbability = 0.1f;

	// symmetrical damage profile with critical hit
	public int GetBulletDamage()
	{
		int damage = 0;
		if(Random.value < criticalHitProbability)
		{
			damage = criticalHitDamage;
		}
		else
		{
			damage = DiceToss.TossDice(numberOfTosses, maxValueOfEachToss);
		}

		return damage;
	}
}
