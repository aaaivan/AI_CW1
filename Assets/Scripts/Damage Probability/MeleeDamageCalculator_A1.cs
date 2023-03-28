using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamageCalculator_A1 : MonoBehaviour, IMeleeDamageCalculator
{
	int numberOfTosses = 3;
	int maxValueOfEachToss = 5;
	int numberOfHits = 2;
	int damageBonus = 9;
	int criticalHitDamage = 25;
	float criticalHitProbability = 0.5f;
	public int GetMeleeDamage()
	{
		int damage = 0;
		for(int i = 0; i < numberOfHits; i++)
		{
			if(Random.value < criticalHitProbability)
			{
				damage += criticalHitDamage;
			}
			else
			{
				damage +=  damageBonus + DiceToss.TossDice(numberOfTosses, maxValueOfEachToss);
			}
		}
		return damage;
	}
}
