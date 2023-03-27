using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingForPlayerState_AI1 : AIState
{
	RandomMovement randomMovement;
	bool superHealingIsHappening = false;

	// Next possible states
	AttackingState_AI1 attackingState;
	SuperHealingState_A1 superHealingState;

	protected override void Awake()
	{
		randomMovement = GetComponent<RandomMovement>();

		attackingState = GetComponent<AttackingState_AI1>();
		superHealingState = GetComponent<SuperHealingState_A1>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(superHealingIsHappening)
		{
			return superHealingState;
		}

		if(CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			return attackingState;
		}
		return null;
	}

	private void PursueSuperHealing(Transform healer)
	{
		superHealingIsHappening = true;
		superHealingState.TargetedHealer = healer;
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		superHealingIsHappening = false;
		SacrificeState_A2.SuperHealing += PursueSuperHealing;

		if (randomMovement != null)
		{
			randomMovement.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		SacrificeState_A2.SuperHealing -= PursueSuperHealing;
		if (randomMovement != null)
		{
			randomMovement.enabled = false;
		}
	}
}
