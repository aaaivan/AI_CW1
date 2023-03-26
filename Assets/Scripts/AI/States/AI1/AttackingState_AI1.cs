using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState_AI1 : AIState
{
	[SerializeField] float stoppingDistanceFromPlayer = 10.0f;
	[SerializeField] float timeBeforePlayerIsLost = 3.0f;
	[SerializeField] float lifeLeftPercentBeforeFleeing = 0.3f;
	float playerLastSeenTime;

	ChaseTarget chasePlayer;
	DamageableObject damageableObject;

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

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(damageableObject.CurrentHealthPercent < lifeLeftPercentBeforeFleeing)
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

	protected override void StateDidBecomeActive(AIState prevState)
	{
		if (chasePlayer != null)
		{
			chasePlayer.Init(player, playerHeight, true, stoppingDistanceFromPlayer);
			chasePlayer.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		if (chasePlayer != null)
		{
			chasePlayer.enabled = false;
		}
	}
}
