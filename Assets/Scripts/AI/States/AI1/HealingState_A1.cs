using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingState_A1 : AIState
{
	[SerializeField] float healtPercentToStopHealing = 0.8f;
	float healthPercentRequiringHealing;

	ChaseTarget followHealer;
	Transform targetedHealer;
	DamageableObject health;

	// next possible states
	SearchingForPlayerState_AI1 searchingForPlayerState;
	AttackingState_AI1 attackingState;
	FleeingState_AI1 fleeingState;
	SearchingForHealerState_AI1 searchingForHealerState;

	protected override void Awake()
	{
		followHealer = GetComponent<ChaseTarget>();
		health = GetComponent<DamageableObject>();

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
		if(health.HealthLeftPercent > healtPercentToStopHealing)
		{
			return searchingForPlayerState;
		}
		if(CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			if(health.HealthLeftPercent < healthPercentRequiringHealing)
			{
				return fleeingState;
			}

			float pDist = Mathf.Pow(health.HealthLeftPercent, 2);
			if(Random.value <= pDist)
			{
				return attackingState;
			}
			else
			{
				return fleeingState;
			}
		}
		return null;
	}

	protected override void StateDidBecomeActive()
	{
		if (followHealer != null)
		{
			Transform healer = null;
			foreach (Transform t in EnemiesManager.Instance.GetEnemiesByFSM("Healer"))
			{
				if (CanSeePoint(t.position + Vector3.up * t.GetComponent<CharacterController>().height, nodeDist))
				{
					healer = t;
					break;
				}
			}

			if(healer != null)
			{
				targetedHealer = healer;
				followHealer.Init(healer, healer.GetComponent<CharacterController>().height, false);
				followHealer.enabled = true;
			}
		}
	}

	protected override void StateDidBecomeInactive()
	{
		if (followHealer != null)
		{
			targetedHealer = null;
			followHealer.enabled = false;
		}
	}
}
