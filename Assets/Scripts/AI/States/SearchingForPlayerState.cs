using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingForPlayerState : AIState
{
	RandomMovement randomMovement;
	protected override void Awake()
	{
		randomMovement = GetComponent<RandomMovement>();
		adjacentStates.Add("attack", GetComponent<AttackingState>());
		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(CanSeePoint(player.position, nodeDist))
		{
			return adjacentStates["attack"];
		}
		return null;
	}

	protected override void StateDidBecomeActive()
	{
		if (randomMovement != null)
		{
			randomMovement.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive()
	{
		if (randomMovement != null)
		{
			randomMovement.enabled = false;
		}
	}
}
