using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMitigationCalculator_Player : MonoBehaviour, IAttackMitigationCalculator
{
	float maxShieldingFactor = 0.1f;
	float perfectShieldingBonus = 0.05f;
	float perfectShieldingProbability = 0.1f;

	public int GetDefenceValue(int attack)
	{
		float defenceMultiplier;
		if(Random.value < perfectShieldingProbability)
		{
			defenceMultiplier = maxShieldingFactor + perfectShieldingBonus;
		}
		else
		{
			defenceMultiplier = Random.value * maxShieldingFactor;
		}
		return Mathf.RoundToInt(defenceMultiplier * attack);
	}
}
