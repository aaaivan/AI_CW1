using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAllyState_AI3 : AIState
{
	RandomMovement randomMovement;
	bool superHealingIsHappening = false;

	// Next possible states
	FleeFromPlayerState_AI3 fleeingState;
	FollowAllyState_AI3 followAllyState;
	SearchPlayerState_AI3 searchPlayerState;
	SuperHealingState_AI3 superHealingState;
	AttackState_A3 attackState;

	protected override void Awake()
	{
		randomMovement = GetComponent<RandomMovement>();

		fleeingState = GetComponent<FleeFromPlayerState_AI3>();
		followAllyState = GetComponent<FollowAllyState_AI3>();
		searchPlayerState = GetComponent<SearchPlayerState_AI3>();
		superHealingState = GetComponent<SuperHealingState_AI3>();
		attackState = GetComponent<AttackState_A3>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if (superHealingIsHappening)
		{
			return superHealingState;
		}

		if (searchPlayerState.WantsToSearchPlayer())
		{
			return searchPlayerState;
		}

		if (CanSeePoint(player.position + Vector3.up * playerHeight, nodeDist))
		{
			if (EnemiesManager.Instance.GetEnemiesByFSM("Pawn").Count < 2)
			{
				return attackState;
			}
			return fleeingState;
		}

		foreach (Transform t in EnemiesManager.Instance.GetEnemiesByFSM("Pawn"))
		{
			if (t == transform) continue;

			if (CanSeePoint(t.position + Vector3.up * t.GetComponent<CharacterController>().height, nodeDist))
			{
				FiniteStateMachine fsm = t.GetComponent<FiniteStateMachine>();
				string currentState = fsm.CurrentState;
				if (currentState == "SearchAlly" || currentState == "SearchPlayer")
				{
					if (t.GetComponent<SearchPlayerState_AI3>().CanAllyJoin(followAllyState))
					{
						followAllyState.TargetedAlly = t.transform;
						return followAllyState;
					}
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
