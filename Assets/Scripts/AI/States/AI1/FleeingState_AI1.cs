using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeingState_AI1 : AIState
{
	[SerializeField] float stopFleeingDelay = 5.0f;
	[SerializeField] float firstSecrificeAttemptDelay = 60.0f;
	[SerializeField] float sacrificeAttemptTimeInterval = 10.0f;
	[SerializeField] float maxHealthPercentAllowingSacrifice = 0.2f;
	float lastTimePlayerWasSeen = 0;
	float timeSinceCriticalHealth = 0;
	float timeSinceLastAttemptedSacrifice = 0;

	FleeFromPlayer fleeFromPlayer;
	DamageableObject health;

	// Next possible states
	SearchingForHealerState_AI1 searchingForHealerState;
	SacrificeState_A1 sacrificeState;


	protected override void Awake()
	{
		fleeFromPlayer = GetComponent<FleeFromPlayer>();
		health = GetComponent<DamageableObject>();

		searchingForHealerState = GetComponent<SearchingForHealerState_AI1>();
		sacrificeState = GetComponent<SacrificeState_A1>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			lastTimePlayerWasSeen = Time.time;
			if (Time.time > timeSinceCriticalHealth + firstSecrificeAttemptDelay && 
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
		if(Time.time >= lastTimePlayerWasSeen + stopFleeingDelay)
		{
			return searchingForHealerState;
		}

		return null;
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		if(fleeFromPlayer != null)
		{
			fleeFromPlayer.enabled = true;
		}
		if (prevState != null && prevState.StateName == "Attack")
		{
			timeSinceCriticalHealth = Time.time;
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		if (fleeFromPlayer != null)
		{
			fleeFromPlayer.enabled = false;
		}
	}
}
