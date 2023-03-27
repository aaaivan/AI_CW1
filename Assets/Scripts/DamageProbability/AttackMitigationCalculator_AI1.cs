using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMitigationCalculator_AI1 : MonoBehaviour, IAttackMitigationCalculator
{
	float maxShieldingFactor = 0.05f;
	float perfectShieldingBonus = 0.05f;
	float perfectShieldingProbability = 0.2f;

	public int GetDefenceValue(int attack)
	{
		float defenceMultiplier = 0;
		if (Random.value < perfectShieldingProbability)
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
