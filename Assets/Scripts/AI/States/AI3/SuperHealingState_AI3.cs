using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperHealingState_AI3 : AIState
{
	ChaseTarget followHealer;
	Transform targetedHealer;
	CharacterController characterController;

	// next state
	SearchAllyState_AI3 searchAllyState;

	public Transform TargetedHealer { set { targetedHealer = value; } }

	protected override void Awake()
	{
		followHealer = GetComponent<ChaseTarget>();
		characterController = GetComponent<CharacterController>();

		searchAllyState = GetComponent<SearchAllyState_AI3>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if (targetedHealer == null)
		{
			return searchAllyState;
		}
		return null;
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		if (followHealer != null)
		{
			if (targetedHealer != null)
			{
				float healingRadius = targetedHealer.GetComponent<SacrificeState_A2>().HealingRadius;
				float targetHealerHeight = targetedHealer.GetComponent<CharacterController>().height;
				float targetHealerRadius = targetedHealer.GetComponent<CharacterController>().radius;
				followHealer.Init(targetedHealer, targetHealerHeight, false,
					targetHealerRadius + characterController.radius, healingRadius - 2 * characterController.radius, true);
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
