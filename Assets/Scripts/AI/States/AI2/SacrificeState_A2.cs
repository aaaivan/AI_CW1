using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeState_A2 : AIState
{
	protected override void Awake()
	{
		base.Awake();
	}

	public override AIState CheckConditions()
	{
		return null;
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		return;
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		return;
	}
}
