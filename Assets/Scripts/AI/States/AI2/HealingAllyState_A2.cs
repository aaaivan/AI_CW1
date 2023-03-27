using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingAllyState_A2 : AIState
{
	[SerializeField] float healingRadius = 5.0f;
	[SerializeField] float healingSpeed = 2.0f;
	[SerializeField] float minHealthToHealAlly = 0.3f;
	float healthAccumulator = 0f;
	bool superHealingIsHappening = false;

	ChaseTarget followAlly;
	DamageableObject health;
	CharacterController characterController;
	HashSet<DamageableObject> alliesToHeal = new HashSet<DamageableObject>();
	HashSet<DamageableObject> alliesWhoSentHealRequest = new HashSet<DamageableObject>();

	// Next possible states
	FleeingState_A2 fleeingState;
	SearchingAllyState_A2 searchingAllyState;
	SuperHealingState_A2 superHealingState;

	public float HealingRadius { get { return healingRadius; } }

	protected override void Awake()
	{
		followAlly = GetComponent<ChaseTarget>();
		health = GetComponent<DamageableObject>();
		characterController = GetComponent<CharacterController>();

		fleeingState = GetComponent<FleeingState_A2>();
		searchingAllyState = GetComponent<SearchingAllyState_A2>();
		superHealingState = GetComponent<SuperHealingState_A2>();

		base.Awake();
	}

	private void Update()
	{
		alliesWhoSentHealRequest.Clear();
		healthAccumulator += Time.deltaTime * healingSpeed;
		int healAmount = Mathf.FloorToInt(healthAccumulator);
		if(healAmount > 0)
		{
			healthAccumulator -= healAmount;
			foreach(DamageableObject ally in alliesToHeal)
			{
				if (Vector3.Distance(ally.transform.position, transform.position) < healingRadius)
				{
					ally.Heal(healAmount);
				}
			}
		}
	}

	public override AIState CheckConditions()
	{
		if (superHealingIsHappening)
		{
			return superHealingState;
		}

		if (health.CurrentHealthPercent < minHealthToHealAlly)
		{
			return fleeingState;
		}
		if(alliesToHeal.Count == 0)
		{
			return searchingAllyState;
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
		if (followAlly != null && alliesToHeal.Count > 0)
		{
			followAlly.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		SacrificeState_A2.SuperHealing -= PursueSuperHealing;
		alliesToHeal.Clear();
		alliesWhoSentHealRequest.Clear();
		if (followAlly != null)
		{
			followAlly.enabled = false;
		}
	}

	public bool RequestHealing(DamageableObject requestedBy)
	{
		if (fsm.CurrentState == "SearchAlly" ||
			fsm.CurrentState == this.stateName)
		{
			alliesWhoSentHealRequest.Add(requestedBy);
			return true;
		}
		else if (fsm.CurrentState == "Attack")
		{
			if(Random.value < health.CurrentHealthPercent)
			{
				alliesWhoSentHealRequest.Add(requestedBy);
				return true;
			}
		}
		return false;
	}

	public void StartHealingAlly(DamageableObject ally)
	{
		if(alliesWhoSentHealRequest.Remove(ally))
		{
			alliesToHeal.Add(ally);

			float height = ally.GetComponent<CharacterController>().height;
			followAlly.Init(ally.transform, height, false, healingRadius - characterController.radius * 2);
		}
	}

	public void StopHealingAlly(DamageableObject ally)
	{
		alliesToHeal.Remove(ally);
		alliesWhoSentHealRequest.Remove(ally);
		if(followAlly.Target == ally && alliesToHeal.Count > 0)
		{
			var it = alliesToHeal.GetEnumerator();
			it.MoveNext();
			Transform newTarget = it.Current.transform;
			float height = newTarget.GetComponent<CharacterController>().height;
			followAlly.Init(newTarget.transform, height, false, healingRadius - characterController.radius * 2);
		}
	}

	public bool IsHealingAlly(DamageableObject ally)
	{
		return alliesToHeal.Contains(ally);
	}

	internal bool WantsToHealAllies()
	{
		return alliesToHeal.Count > 0;
	}
}
