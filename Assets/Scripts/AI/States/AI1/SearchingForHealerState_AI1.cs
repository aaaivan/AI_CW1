using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingForHealerState_AI1 : AIState
{
	[SerializeField] float timeToTriggerSacrifice = 60.0f;
	float lastHealedTime = 0;
	int healthWhenStartingSearch = 0;

	RandomMovement randomMovement;
	DamageableObject health;

	// Next possible states
	HealingState_A1 healingState;
	FleeingState_AI1 fleeingState;
	SacrificeState_A1 sacrificeState;

	protected override void Awake()
	{
		randomMovement = GetComponent<RandomMovement>();
		health = GetComponent<DamageableObject>();

		healingState = GetComponent<HealingState_A1>();
		fleeingState = GetComponent<FleeingState_AI1>();
		sacrificeState = GetComponent<SacrificeState_A1>();

		base.Awake();
	}

	public override AIState CheckConditions()
	{
		if(CanSeePoint(player.position + Vector3.up * playerHeight, nodeDist))
		{
			if(Time.time > lastHealedTime + timeToTriggerSacrifice)
			{
				float probDistr = 1 - Mathf.Sqrt(health.HealthLeftPercent);
				if(Random.value < probDistr)
				{
					return sacrificeState;
				}
				else
				{
					return fleeingState;
				}
			}

			return fleeingState;
		}
		foreach(Transform t in EnemiesManager.Instance.GetEnemiesByFSM("Healer"))
		{
			if (CanSeePoint(t.position + Vector3.up * t.GetComponent<CharacterController>().height, nodeDist))
			{
				healthWhenStartingSearch = 0;
				return healingState;
			}
		}
		return null;
	}

	protected override void StateDidBecomeActive()
	{
		if(randomMovement != null)
		{
			randomMovement.enabled = true;
			if(health.CurrentHealth > healthWhenStartingSearch)
			{
				lastHealedTime = Time.time;
				healthWhenStartingSearch = health.CurrentHealth;
			}
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
