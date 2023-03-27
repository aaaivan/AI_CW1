using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState_AI1 : AIState
{
	[SerializeField] float stoppingDistanceFromPlayer = 10.0f;
	[SerializeField] float timeBeforePlayerIsLost = 3.0f;
	[SerializeField] float lifeLeftPercentBeforeFleeing = 0.3f;
	float playerLastSeenTime;
	bool superHealingIsHappening = false;

	ChaseTarget chasePlayer;
	DamageableObject damageableObject;
	SuperHealingState_A1 superHealingState;

	// Next possible states
	SearchingForPlayerState_AI1 searchingForPlayerState;
	FleeingState_AI1 fleeingState;

	public float LifeLeftPercentBeforeFleeing { get { return lifeLeftPercentBeforeFleeing; } }

	protected override void Awake()
	{
		chasePlayer = GetComponent<ChaseTarget>();
		damageableObject = GetComponent<DamageableObject>();

		searchingForPlayerState = GetComponent<SearchingForPlayerState_AI1>();
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

		if (damageableObject.CurrentHealthPercent < lifeLeftPercentBeforeFleeing)
		{
			return fleeingState;
		}

		if (CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			playerLastSeenTime = Time.time;
		}
		else if(Time.time >= playerLastSeenTime + timeBeforePlayerIsLost)
		{
			return searchingForPlayerState;
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
			chasePlayer.Init(player, playerHeight, true, stoppingDistanceFromPlayer);
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
