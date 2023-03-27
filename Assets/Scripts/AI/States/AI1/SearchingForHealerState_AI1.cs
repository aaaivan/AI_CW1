using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingForHealerState_AI1 : AIState
{
	RandomMovement randomMovement;
	DamageableObject health;
	bool superHealingIsHappening = false;


	// Next possible states
	HealingState_A1 healingState;
	FleeingState_AI1 fleeingState;
	SuperHealingState_A1 superHealingState;

	protected override void Awake()
	{
		randomMovement = GetComponent<RandomMovement>();
		health = GetComponent<DamageableObject>();

		healingState = GetComponent<HealingState_A1>();
		fleeingState = GetComponent<FleeingState_AI1>();
		superHealingState = GetComponent<SuperHealingState_A1>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if (superHealingIsHappening)
		{
			return superHealingState;
		}

		if (CanSeePoint(player.position + Vector3.up * playerHeight, nodeDist))
		{
			return fleeingState;
		}
		foreach(Transform t in EnemiesManager.Instance.GetEnemiesByFSM("Healer"))
		{
			if (CanSeePoint(t.position + Vector3.up * t.GetComponent<CharacterController>().height, nodeDist))
			{
				FiniteStateMachine fsm = t.GetComponent<FiniteStateMachine>();
				if(((HealingAllyState_A2)fsm.GetStateByName("HealAlly")).RequestHealing(health))
				{
					healingState.TargetedHealer = t;
					return healingState;
				}
			}
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
