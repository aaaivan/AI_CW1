using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperHealingState_A2 : AIState
{
	ChaseTarget followHealer;
	Transform targetedHealer;

	// next state
	HealingState_A2 healingState;

	public Transform TargetedHealer { set { targetedHealer = value; } }

	protected override void Awake()
	{
		followHealer = GetComponent<ChaseTarget>();

		healingState = GetComponent<HealingState_A2>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(targetedHealer == null)
		{
			return healingState;
		}
		return null;
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		if (followHealer != null)
		{
			if (targetedHealer != null)
			{
				followHealer.Init(targetedHealer, targetedHealer.GetComponent<CharacterController>().height,
					false, targetedHealer.GetComponent<SacrificeState_A2>().HealingRadius / 2);
				followHealer.enabled = true;
			}
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		if (targetedHealer != null)
		{
			targetedHealer = null;
		}
		if (followHealer != null)
		{
			followHealer.enabled = false;
		}
	}
}
