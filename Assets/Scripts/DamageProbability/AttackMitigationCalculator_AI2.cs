using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMitigationCalculator_AI2 : MonoBehaviour, IAttackMitigationCalculator
{
	float maxShieldingFactor = 0.15f;
	float perfectShieldingBonus = 0.1f;
	float perfectShieldingProbability = 0.1f;

	public void ActivateShieldPowerup(float powerupDuration)
	{
		return;
	}

	public int GetDefenceValue(int attack)
	{
		float defenceMultiplier;
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
