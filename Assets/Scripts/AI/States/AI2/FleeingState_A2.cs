using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeingState_A2 : AIState
{
	[SerializeField] float stopFleeingDelay = 3.0f;
	[SerializeField] float firstSecrificeAttemptDelay = 60.0f;
	[SerializeField] float sacrificeAttemptTimeInterval = 10.0f;
	[SerializeField] float maxHealthPercentAllowingSacrifice = 0.2f;
	[SerializeField] float healthPercentRequiringHealing = 0.7f;
	float lastTimePlayerWasSeen = 0;
	float timeFleeStarted = 0;
	float timeSinceLastAttemptedSacrifice = 0;

	FleeFromPlayer fleeFromPlayer;
	DamageableObject health;

	// Next possible states
	HealingState_A2 healingState;
	SearchingAllyState_A2 searchingAllyState;
	SacrificeState_A2 sacrificeState;


	protected override void Awake()
	{
		fleeFromPlayer = GetComponent<FleeFromPlayer>();
		health = GetComponent<DamageableObject>();

		healingState = GetComponent<HealingState_A2>();
		searchingAllyState = GetComponent<SearchingAllyState_A2>();
		sacrificeState = GetComponent<SacrificeState_A2>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if (CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			lastTimePlayerWasSeen = Time.time;
			if (Time.time > timeFleeStarted + firstSecrificeAttemptDelay &&
				Time.time > timeSinceLastAttemptedSacrifice + sacrificeAttemptTimeInterval &&
				health.CurrentHealthPercent < maxHealthPercentAllowingSacrifice)
			{
				float probDistr = 1 - Mathf.Sqrt(health.CurrentHealthPercent);
				timeSinceLastAttemptedSacrifice = Time.time;
				if (Random.value < probDistr)
				{
					return sacrificeState;
				}
			}
		}
		if (Time.time >= lastTimePlayerWasSeen + stopFleeingDelay)
		{
			if(health.CurrentHealthPercent > healthPercentRequiringHealing)
			{
				float probDist = Mathf.Pow(health.CurrentHealthPercent, 2);
				if (Random.value < probDist)
				{
					return searchingAllyState;
				}
			}
			return healingState;
		}
		return null;
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		if (fleeFromPlayer != null)
		{
			fleeFromPlayer.enabled = true;
		}
		timeFleeStarted = Time.time;
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		if (fleeFromPlayer != null)
		{
			fleeFromPlayer.enabled = false;
		}
	}
}
