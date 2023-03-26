using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingState_A1 : AIState
{
	[SerializeField] float healthPercentToStopHealing = 0.8f;
	[SerializeField] float stopHealingAtMinHealthProbability = 0.5f;
	[SerializeField] float tryStopHealingTimeInterval = 5.0f;
	float lastStopHealingAttemptTime = 0;
	float healthPercentRequiringHealing;

	ChaseTarget followHealer;
	Transform targetedHealer;
	DamageableObject health;
	CharacterController characterController;

	// next possible states
	SearchingForPlayerState_AI1 searchingForPlayerState;
	AttackingState_AI1 attackingState;
	FleeingState_AI1 fleeingState;
	SearchingForHealerState_AI1 searchingForHealerState;

	public Transform TargetedHealer { set { targetedHealer = value; } }

	protected override void Awake()
	{
		followHealer = GetComponent<ChaseTarget>();
		health = GetComponent<DamageableObject>();
		characterController = GetComponent<CharacterController>();

		searchingForPlayerState = GetComponent<SearchingForPlayerState_AI1>();
		attackingState = GetComponent<AttackingState_AI1>();
		fleeingState = GetComponent<FleeingState_AI1>();
		searchingForHealerState = GetComponent<SearchingForHealerState_AI1>();

		base.Awake();
	}

	private void Start()
	{
		healthPercentRequiringHealing = GetComponent<AttackingState_AI1>().LifeLeftPercentBeforeFleeing;
	}

	public override AIState CheckConditions()
	{
		if(targetedHealer == null)
		{
			return searchingForHealerState;
		}
		HealingAllyState_A2 healer = targetedHealer.GetComponent<HealingAllyState_A2>();
		if(!healer.IsHealingAlly(health) || !healer.enabled)
		{
			return searchingForHealerState;
		}
		if (health.CurrentHealthPercent > healthPercentToStopHealing &&
			Time.time >= lastStopHealingAttemptTime + tryStopHealingTimeInterval)
		{
			lastStopHealingAttemptTime = Time.time;
			float probDistr = health.CurrentHealthPercent - healthPercentToStopHealing;
			probDistr /= (1 - healthPercentToStopHealing);
			probDistr *= (1 - stopHealingAtMinHealthProbability);
			probDistr += stopHealingAtMinHealthProbability;
			if (Random.value < probDistr)
			{
				return searchingForPlayerState;
			}
		}
		if(CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			if(health.CurrentHealthPercent > healthPercentRequiringHealing)
			{
				float pDist = Mathf.Pow(health.CurrentHealthPercent, 2);
				if (Random.value <= pDist)
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
		if (followHealer != null)
		{
			if(targetedHealer != null)
			{
				followHealer.Init(targetedHealer, targetedHealer.GetComponent<CharacterController>().height,
					false, targetedHealer.GetComponent<HealingAllyState_A2>().HealingRadius - characterController.radius * 2);
				followHealer.enabled = true;
				targetedHealer.GetComponent<HealingAllyState_A2>().StartHealingAlly(health);
			}
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		if(targetedHealer != null)
		{
			targetedHealer.GetComponent<HealingAllyState_A2>().StopHealingAlly(health);
			targetedHealer = null;
		}
		if (followHealer != null)
		{
			followHealer.enabled = false;
		}
	}
}
