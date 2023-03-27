using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingState_A2 : AIState
{
	[SerializeField] float minHealthPercentToRecover = 0.8f;
	[SerializeField] float stopHealingAtMinHealthProbability = 0.5f;
	[SerializeField] float tryStopHealingTimeInterval = 5.0f;
	[SerializeField] float healingSpeed = 2;
	float healthAccumulator = 0;
	float lastStopHealingAtemptTime = 0;

	DamageableObject health;
	bool superHealingIsHappening = false;

	// Next possible states
	FleeingState_A2 fleeingState;
	SearchingAllyState_A2 searchingAllyState;
	SuperHealingState_A2 superHealingState;

	protected override void Awake()
	{
		health = GetComponent<DamageableObject>();

		fleeingState = GetComponent<FleeingState_A2>();
		searchingAllyState = GetComponent<SearchingAllyState_A2>();
		superHealingState = GetComponent<SuperHealingState_A2>();

		base.Awake();
	}

	private void Update()
	{
		healthAccumulator += Time.deltaTime * healingSpeed;
		int recoveredHP = Mathf.FloorToInt(healthAccumulator);
		if (recoveredHP > 0)
		{
			health.Heal(recoveredHP);
			healthAccumulator -= recoveredHP;
		}
	}

	public override AIState CheckConditions()
	{
		if (superHealingIsHappening)
		{
			return superHealingState;
		}

		if (CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			return fleeingState;
		}
		if(health.CurrentHealthPercent > minHealthPercentToRecover &&
			Time.time >= lastStopHealingAtemptTime + tryStopHealingTimeInterval)
		{
			lastStopHealingAtemptTime = Time.time;
			float probDistr = health.CurrentHealthPercent - minHealthPercentToRecover;
			probDistr /= (1 - minHealthPercentToRecover);
			probDistr *= (1 - stopHealingAtMinHealthProbability);
			probDistr += stopHealingAtMinHealthProbability;
			if(Random.value < probDistr)
			{
				return searchingAllyState;
			}
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
		healthAccumulator = 0;
		return;
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		SacrificeState_A2.SuperHealing -= PursueSuperHealing;
		return;
	}
}
