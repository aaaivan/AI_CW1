using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamageCalculator_AI2 : MonoBehaviour, IBulletDamageCalculator
{
	int numberOfTosses = 1;
	int maxValueOfEachToss = 24;
	int criticalHitDamage = 25;
	float criticalHitProbability = 0.25f;

	// damage profile skwed to the left with critical hit
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
			int maxDamage = 0;
			for(int i = 0; i < numberOfSamples; i++)
			{
				int t = DiceToss.TossDice(numberOfTosses, maxValueOfEachToss);
				total += t;
				if(t > maxDamage)
				{
					maxDamage = t;
				}
			}
			// remove the highest value and avarage the other two
			damage = Mathf.RoundToInt((float)(total - maxDamage) / (numberOfSamples - 1));
		}

		return damage;
	}
}
