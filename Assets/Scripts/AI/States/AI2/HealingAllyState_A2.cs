using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingAllyState_A2 : AIState
{
	[SerializeField] float healingRadius = 5.0f;
	[SerializeField] float healingSpeed = 2.0f;
	[SerializeField] float minHealthToHealAlly = 0.3f;
	[SerializeField] float minHealthToStopHealingSelfToHealAlly = 0.5f;
	float healthAccumulator = 0f;
	bool superHealingIsHappening = false;

	ChaseTarget followAlly;
	DamageableObject health;
	CharacterController characterController;
	List<DamageableObject> alliesToHeal = new List<DamageableObject>();

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
		bool newFollowTarget = followAlly.Target == null;
		for(int i = alliesToHeal.Count - 1; i >= 0; --i)
		{
			DamageableObject ally = alliesToHeal[i];
			if(ally == null)
			{
				alliesToHeal.RemoveAt(i);
			}
			else if(ally.GetComponent<FiniteStateMachine>().CurrentState != "Heal")
			{
				alliesToHeal.RemoveAt(i);
				if(followAlly.Target == ally)
				{
					newFollowTarget = true;
				}
			}
		}

		if(newFollowTarget && alliesToHeal.Count > 0)
		{
			Transform newTarget = alliesToHeal[0].transform;
			float height = newTarget.GetComponent<CharacterController>().height;
			float stoppingDist = healingRadius - characterController.radius * 2;
			followAlly.Init(newTarget.transform, height, false, 0, stoppingDist, false);
		}

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
			if (alliesToHeal.Count > 0)
			{
				Transform newTarget = alliesToHeal[0].transform;
				float height = newTarget.GetComponent<CharacterController>().height;
				float stoppingDist = healingRadius - characterController.radius * 2;
				followAlly.Init(newTarget.transform, height, false, 0, stoppingDist, false);
			}
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		SacrificeState_A2.SuperHealing -= PursueSuperHealing;
		alliesToHeal.Clear();
		if (followAlly != null)
		{
			followAlly.enabled = false;
		}
	}

	public bool RequestHealing(DamageableObject requestedBy)
	{
		if (fsm.CurrentState == "SearchAlly" ||
			fsm.CurrentState == "Attack" ||
			fsm.CurrentState == this.stateName)
		{
			alliesToHeal.Add(requestedBy);
			return true;
		}
		else if (fsm.CurrentState == "HealSelf" &&
			health.CurrentHealthPercent > minHealthToStopHealingSelfToHealAlly)
		{
			alliesToHeal.Add(requestedBy);
			return true;
		}

		return false;
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
