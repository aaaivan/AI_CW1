using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMitigationCalculator_Player : MonoBehaviour, IAttackMitigationCalculator
{
	float maxShieldingFactor = 0.1f;
	float maxShieldingFactorWithPowerup = 0.5f;
	float perfectShieldingBonus = 0.05f;
	float perfectShieldingBonusWithPowerup = 0.05f;
	float perfectShieldingProbability = 0.1f;
	float perfectShieldingProbabilityWithPowerup = 0.2f;

	float timeShieldPowerupWasActivated = 0;
	float shieldPowerupDuration = 0;

	public int GetDefenceValue(int attack)
	{
		bool shieldActive = Time.time < timeShieldPowerupWasActivated + shieldPowerupDuration;
		float defenceMultiplier;

		float _perfectShieldingProbability = shieldActive ? perfectShieldingProbabilityWithPowerup : perfectShieldingProbability;
		float _maxShieldingFactor = shieldActive ? maxShieldingFactorWithPowerup : maxShieldingFactor;
		float _perfectShieldingBonus = shieldActive ? perfectShieldingBonusWithPowerup : perfectShieldingBonus;

		if (Random.value < _perfectShieldingProbability)
		{
			defenceMultiplier = _maxShieldingFactor + _perfectShieldingBonus;
		}
		else
		{
			defenceMultiplier = Random.value * _maxShieldingFactor;
		}
		return Mathf.RoundToInt(defenceMultiplier * attack);
	}

	public void ActivateShieldPowerup(float powerupDuration)
	{
		timeShieldPowerupWasActivated = Time.time;
		shieldPowerupDuration = powerupDuration;
	}
}
