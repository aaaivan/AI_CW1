using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackMitigationCalculator
{
	void ActivateShieldPowerup(float powerupDuration);
	public int GetDefenceValue(int attack);
}
