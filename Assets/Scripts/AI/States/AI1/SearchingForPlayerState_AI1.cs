using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingForPlayerState_AI1 : AIState
{
	RandomMovement randomMovement;

	// Next possible states
	AttackingState_AI1 attackingState;

	protected override void Awake()
	{
		randomMovement = GetComponent<RandomMovement>();

		attackingState = GetComponent<AttackingState_AI1>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			return attackingState;
		}
		return null;
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		if (randomMovement != null)
		{
			randomMovement.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		if (randomMovement != null)
		{
			randomMovement.enabled = false;
		}
	}
}
