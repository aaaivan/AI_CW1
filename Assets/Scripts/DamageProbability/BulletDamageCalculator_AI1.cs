using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamageCalculator_AI1 : MonoBehaviour, IBulletDamageCalculator
{
	int numberOfTosses = 1;
	int maxValueOfEachToss = 24;
	int criticalHitDamage = 25;
	float criticalHitProbability = 0.05f;

	// damage profile skwed to the right with critical hit
	public int GetBulletDamage()
	{
		int damage = 0;
		if (Random.value < criticalHitProbability)
		{
			damage = criticalHitDamage;
		}
		else
		{
			int numberOfSamples = 3;
			int total = 0;
			int minDamage = int.MaxValue;
			for (int i = 0; i < numberOfSamples; i++)
			{
				int t = DiceToss.TossDice(numberOfTosses, maxValueOfEachToss);
				total += t;
				if (t < minDamage)
				{
					minDamage = t;
				}
			}
			// remove the lowest value and avarage th eremaining two
			damage = Mathf.RoundToInt((float)(total - minDamage) / (numberOfSamples - 1));
		}

		return damage;
	}
}
