using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeFromPlayerState_AI3 : AIState
{
	[SerializeField] float stopFleeingDelay = 5.0f;
	float lastTimePlayerWasSeen = 0;
	bool superHealingIsHappening = false;

	FleeFromPlayer fleeFromPlayer;

	// Next possible states
	SearchAllyState_AI3 searchAllyState;
	SuperHealingState_AI3 superHealingState;

	protected override void Awake()
	{
		fleeFromPlayer = GetComponent<FleeFromPlayer>();

		searchAllyState = GetComponent<SearchAllyState_AI3>();
		superHealingState = GetComponent<SuperHealingState_AI3>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if (superHealingIsHappening)
		{
			return superHealingState;
		}

		if (CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			lastTimePlayerWasSeen = Time.time;
		}

		if (Time.time >= lastTimePlayerWasSeen + stopFleeingDelay)
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
		if (fleeFromPlayer != null)
		{
			fleeFromPlayer.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		SacrificeState_A2.SuperHealing -= PursueSuperHealing;
		if (fleeFromPlayer != null)
		{
			fleeFromPlayer.enabled = false;
		}
	}
}
