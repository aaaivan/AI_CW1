using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeingState_AI1 : AIState
{
	[SerializeField] float stopFleeingDelay = 5.0f;
	float lastTimePlayerWasSeen = 0;

	FleeFromPlayer fleeFromPlayer;

	SearchingForHealerState_AI1 searchingForHealerState;

	protected override void Awake()
	{
		fleeFromPlayer = GetComponent<FleeFromPlayer>();

		searchingForHealerState = GetComponent<SearchingForHealerState_AI1>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			lastTimePlayerWasSeen = Time.time;
		}
		if(Time.time >= lastTimePlayerWasSeen + stopFleeingDelay)
		{
			return searchingForHealerState;
		}

		return null;
	}

	protected override void StateDidBecomeActive()
	{
		if(fleeFromPlayer != null)
		{
			fleeFromPlayer.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive()
	{
		if (fleeFromPlayer != null)
		{
			fleeFromPlayer.enabled = false;
		}
	}
}
