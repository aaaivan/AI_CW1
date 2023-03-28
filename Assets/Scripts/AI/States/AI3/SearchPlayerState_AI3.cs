using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPlayerState_AI3 : AIState
{
	[SerializeField] int maxFormationSize = 5;
	[SerializeField] int minFormationSize = 3;

	RandomMovement randomMovement;
	bool superHealingIsHappening = false;

	List<FollowAllyState_AI3> alliesBeingLed = new List<FollowAllyState_AI3>();

	// Next possible states
	AttackState_A3 attackState;
	SuperHealingState_AI3 superHealingState;
	SearchAllyState_AI3 searchAllyState;
	FleeFromPlayerState_AI3 fleeFromPlayerState;

	protected override void Awake()
	{
		randomMovement = GetComponent<RandomMovement>();

		attackState = GetComponent<AttackState_A3>();
		superHealingState = GetComponent<SuperHealingState_AI3>();
		searchAllyState = GetComponent<SearchAllyState_AI3>();
		fleeFromPlayerState = GetComponent<FleeFromPlayerState_AI3>();

		base.Awake();
	}

	private void Update()
	{
		for (int i = alliesBeingLed.Count - 1; i >= 0; --i)
		{
			var ally = alliesBeingLed[i];
			if (ally == null)
			{
				alliesBeingLed.RemoveAt(i);
			}
			else if (ally.GetComponent<FiniteStateMachine>().CurrentState != "FollowAlly")
			{
				alliesBeingLed.RemoveAt(i);
			}
		}
	}

	public override AIState CheckConditions()
	{
		if (superHealingIsHappening)
		{
			return superHealingState;
		}

		if (CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			bool attack = alliesBeingLed.Count + 1 >= minFormationSize
				|| EnemiesManager.Instance.GetEnemiesByFSM("Pawn").Count < maxFormationSize;
			foreach (var ally in alliesBeingLed)
			{
				if(ally != null)
				{
					if(attack)
					{
						ally.TriggerAttack();
					}
					else
					{
						ally.TriggetFlee();
					}
				}
			}
			if(attack)
			{
				return attackState;
			}
			else
			{
				return fleeFromPlayerState;
			}
		}

		if(alliesBeingLed.Count == 0)
		{
			return searchAllyState;
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
		alliesBeingLed.Clear();
		if (randomMovement != null)
		{
			randomMovement.enabled = false;
		}
	}

	internal bool WantsToSearchPlayer()
	{
		return alliesBeingLed.Count > 0;
	}

	internal bool CanAllyJoin(FollowAllyState_AI3 ally)
	{
		if (fsm.CurrentState == "SearchAlly" ||
			fsm.CurrentState == this.stateName &&
			alliesBeingLed.Count < maxFormationSize)
		{
			alliesBeingLed.Add(ally);
			return true;
		}

		return false;
	}

	internal bool IsLeadingAlly(FollowAllyState_AI3 ally)
	{
		return alliesBeingLed.Contains(ally);
	}
}
