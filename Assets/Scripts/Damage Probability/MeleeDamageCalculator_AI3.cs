using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamageCalculator_AI3 : MonoBehaviour, IMeleeDamageCalculator
{
	int numberOfTosses = 3;
	int maxValueOfEachToss = 4;
	int criticalHitDamage = 15;
	float criticalHitProbability = 0.1f;
	public int GetMeleeDamage()
	{
		int damage = 0;

		if (Random.value < criticalHitProbability)
		{
			damage += criticalHitDamage;
		}
		else
		{
			damage += DiceToss.TossDice(numberOfTosses, maxValueOfEachToss);
		}

		return damage;
	}
}
