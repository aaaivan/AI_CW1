using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAllyState_AI3 : AIState
{
	[SerializeField] float formationRadius = 5f;
	bool superHealingIsHappening = false;

	ChaseTarget followAlly;
	Transform leaderAlly;
	CharacterController characterController;
	bool attackPlayer = false;
	bool flee = false;

	// next possible states
	SearchAllyState_AI3 searchAllyState;
	AttackState_A3 attackState;
	SuperHealingState_AI3 superHealingState;
	FleeFromPlayerState_AI3 fleeFromPlayerState;

	public Transform TargetedAlly { set { leaderAlly = value; } }

	protected override void Awake()
	{
		followAlly = GetComponent<ChaseTarget>();
		characterController = GetComponent<CharacterController>();

		searchAllyState = GetComponent<SearchAllyState_AI3>();
		attackState = GetComponent<AttackState_A3>();
		superHealingState = GetComponent<SuperHealingState_AI3>();
		fleeFromPlayerState = GetComponent<FleeFromPlayerState_AI3>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if (superHealingIsHappening)
		{
			return superHealingState;
		}

		if (attackPlayer)
		{
			return attackState;
		}

		if(flee)
		{

		}

		if (leaderAlly == null)
		{
			return searchAllyState;
		}
		SearchPlayerState_AI3 leader = leaderAlly.GetComponent<SearchPlayerState_AI3>();
		if(leader.enabled == false || !leader.IsLeadingAlly(this))
		{
			return searchAllyState;
		}



		return null;
	}

	public void TriggerAttack()
	{
		attackPlayer = true;
	}

	internal void TriggetFlee()
	{
		flee = true;
	}

	private void PursueSuperHealing(Transform healer)
	{
		superHealingIsHappening = true;
		superHealingState.TargetedHealer = healer;
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		superHealingIsHappening = false;
		attackPlayer = false;
		flee = false;
		SacrificeState_A2.SuperHealing += PursueSuperHealing;
		if (followAlly != null)
		{
			if (leaderAlly != null)
			{
				float targetLeaderHeight = leaderAlly.GetComponent<CharacterController>().height;
				float targetLeaderRadius = leaderAlly.GetComponent<CharacterController>().radius;
				followAlly.Init(leaderAlly, targetLeaderHeight, false,
					targetLeaderRadius + characterController.radius, formationRadius, true);
				followAlly.enabled = true;
			}
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		SacrificeState_A2.SuperHealing -= PursueSuperHealing;
		if (leaderAlly != null)
		{
			leaderAlly = null;
		}
		if (followAlly != null)
		{
			followAlly.enabled = false;
		}
	}
}
