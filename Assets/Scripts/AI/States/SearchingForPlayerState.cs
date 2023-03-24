using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingForPlayerState : AIState
{
	protected override void Awake()
	{
		adjacentStates.Add("attack", GetComponent<AttackingState>());
		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(CanSeePoint(player.position))
		{
			return adjacentStates["attack"];
		}
		return null;
	}
}
