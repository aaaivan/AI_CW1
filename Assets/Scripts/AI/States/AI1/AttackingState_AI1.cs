using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState_AI1 : AIState
{
	[SerializeField] float timeBeforePlayerIsLost = 3.0f;
	[SerializeField] float lifeLeftPercentBeforeFleeing = 0.3f;
	[SerializeField] float attemptFleeTimeInterval = 10.0f;
	float playerLastSeenTime;
	float lastFleeAttemptTime;
	bool superHealingIsHappening = false;
	float minStoppingDistFromPlayer = 6.0f;
	float maxStoppingDistFromPlayer = 10.0f;

	ChaseTarget chasePlayer;
	DamageableObject health;

	// Next possible states
	SearchingForPlayerState_AI1 searchingForPlayerState;
	FleeingState_AI1 fleeingState;
	SuperHealingState_A1 superHealingState;

	public float LifeLeftPercentBeforeFleeing { get { return lifeLeftPercentBeforeFleeing; } }

	protected override void Awake()
	{
		chasePlayer = GetComponent<ChaseTarget>();
		health = GetComponent<DamageableObject>();

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

		if (health.CurrentHealthPercent < lifeLeftPercentBeforeFleeing)
		{
			return fleeingState;
		}
		else if (Time.time > lastFleeAttemptTime + attemptFleeTimeInterval)
		{
			lastFleeAttemptTime = Time.time;
			float probDistr = health.CurrentHealthPercent - lifeLeftPercentBeforeFleeing;
			probDistr /= (1 - lifeLeftPercentBeforeFleeing);
			probDistr = 1 - probDistr;
			probDistr = Mathf.Sqrt(Mathf.Clamp01(probDistr));
			if (Random.value < probDistr)
			{
				return fleeingState;
			}
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
		lastFleeAttemptTime = Time.time;
		playerLastSeenTime = Time.time;
		superHealingIsHappening = false;
		SacrificeState_A2.SuperHealing += PursueSuperHealing;
		if (chasePlayer != null)
		{
			chasePlayer.Init(player, playerHeight, true, minStoppingDistFromPlayer, maxStoppingDistFromPlayer, true);
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
