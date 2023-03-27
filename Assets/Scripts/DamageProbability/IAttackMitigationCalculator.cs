using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackMitigationCalculator
{
	public int GetDefenceValue(int attack);
}
