using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : AIState
{
	[SerializeField] float timeBeforePlayerIsLost = 5.0f;
	[SerializeField] float lifeLeftPercentBeforeFleeing = 30.0f;
	float playerLastSeenTime;

	ChasePlayer chasePlayer;
	DamageableObject damageableObject;
	protected override void Awake()
	{
		chasePlayer = GetComponent<ChasePlayer>();
		damageableObject = GetComponent<DamageableObject>();

		adjacentStates.Add("search", GetComponent<SearchingForPlayerState>());
		adjacentStates.Add("flee", GetComponent<FleeingState>());

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(damageableObject.LifeLeftPercent <= lifeLeftPercentBeforeFleeing)
		{
			return adjacentStates["flee"];
		}

		if (CanSeePoint(player.position, nodeDist))
		{
			playerLastSeenTime = Time.time;
		}
		else if(Time.time >= playerLastSeenTime + timeBeforePlayerIsLost)
		{
			return adjacentStates["search"];
		}

		return null;
	}

	protected override void StateDidBecomeActive()
	{
		if (chasePlayer != null)
		{
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
