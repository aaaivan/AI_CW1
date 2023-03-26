using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState_AI1 : AIState
{
	[SerializeField] float timeBeforePlayerIsLost = 5.0f;
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
		if(damageableObject.HealthLeftPercent <= lifeLeftPercentBeforeFleeing)
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

	protected override void StateDidBecomeActive()
	{
		if (chasePlayer != null)
		{
			chasePlayer.Init(player, playerHeight, true);
			chasePlayer.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive()
	{
		if (chasePlayer != null)
		{
			chasePlayer.enabled = false;
		}
	}
}
