using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState_A2 : AIState
{
	[SerializeField] float stoppingDistFromPlayer = 10.0f;
	[SerializeField] float timeBeforePlayerIsLost = 3.0f;
	[SerializeField] float lifeLeftPercentBeforeFleeing = 0.5f;
	float playerLastSeenTime;
	bool superHealingIsHappening = false;

	ChaseTarget chasePlayer;
	DamageableObject health;

	// Next possible states
	SearchingAllyState_A2 searchingForAlly;
	FleeingState_A2 fleeingState;
	HealingAllyState_A2 healingAlly;
	SuperHealingState_A2 superHealingState;

	public float LifeLeftPercentBeforeFleeing { get { return lifeLeftPercentBeforeFleeing; } }

	protected override void Awake()
	{
		chasePlayer = GetComponent<ChaseTarget>();
		health = GetComponent<DamageableObject>();

		searchingForAlly = GetComponent<SearchingAllyState_A2>();
		fleeingState = GetComponent<FleeingState_A2>();
		healingAlly = GetComponent<HealingAllyState_A2>();
		superHealingState = GetComponent<SuperHealingState_A2>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if (superHealingIsHappening)
		{
			return superHealingState;
		}

		if (healingAlly.WantsToHealAllies())
		{
			return healingAlly;
		}

		if (health.CurrentHealthPercent < lifeLeftPercentBeforeFleeing)
		{
			return fleeingState;
		}

		if (CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			playerLastSeenTime = Time.time;
		}
		else if (Time.time >= playerLastSeenTime + timeBeforePlayerIsLost)
		{
			return searchingForAlly;
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
		if (chasePlayer != null)
		{
			chasePlayer.Init(player, playerHeight, true, stoppingDistFromPlayer);
			chasePlayer.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		SacrificeState_A2.SuperHealing -= PursueSuperHealing;
		if (chasePlayer != null)
		{
			chasePlayer.enabled = false;
		}
	}
}
