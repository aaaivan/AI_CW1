using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingAllyState_A2 : AIState
{
	[SerializeField] float minHealthPercentToAttack = 0.5f;
	[SerializeField] float attackProbabilityAtFullHealth = 0.5f;

	RandomMovement randomMovement;
	DamageableObject health;

	// Next possible states
	HealingAllyState_A2 healingAlly;
	FleeingState_A2 fleeingState;
	AttackingState_A2 attackingState;

	protected override void Awake()
	{
		randomMovement = GetComponent<RandomMovement>();
		health = GetComponent<DamageableObject>();

		healingAlly = GetComponent<HealingAllyState_A2>();
		fleeingState = GetComponent<FleeingState_A2>();
		attackingState = GetComponent<AttackingState_A2>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if (healingAlly.WantsToHealAllies())
		{
			return healingAlly;
		}

		if (CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			if(health.CurrentHealthPercent > minHealthPercentToAttack)
			{
				float probDist = Mathf.Pow(health.CurrentHealthPercent - minHealthPercentToAttack, 2);
				probDist /= Mathf.Pow(1f - minHealthPercentToAttack, 2);
				probDist *= attackProbabilityAtFullHealth;
				if(Random.value < probDist)
				{
					return attackingState;
				}
			}
			return fleeingState;
		}

		return null;
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		if(randomMovement != null)
		{
			randomMovement.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		if (randomMovement != null)
		{
			randomMovement.enabled = false;
		}
	}
}
