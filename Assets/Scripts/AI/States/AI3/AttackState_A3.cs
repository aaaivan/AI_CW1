using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_A3 : AIState
{
	[SerializeField] float timeBeforePlayerIsLost = 3.0f;
	[SerializeField] float attackDuration = 5.0f;
	float playerLastSeenTime = 0;
	float attackStartTime = 0;
	bool attackStarted = false;
	bool superHealingIsHappening = false;
	float minStoppingDistFromPlayer = 1.0f;
	float maxStoppingDistFromPlayer = 3.0f;
	float shockwaveRadius = 4.0f;

	ChaseTarget chasePlayer;
	MeleeAttack meleeAttack;
	Transform shockwaveVFX;

	// Next possible states
	SearchAllyState_AI3 searchAllyState;
	FleeFromPlayerState_AI3 fleeingState;
	SuperHealingState_AI3 superHealingState;

	protected override void Awake()
	{
		chasePlayer = GetComponent<ChaseTarget>();
		meleeAttack = GetComponent<MeleeAttack>();
		shockwaveVFX = transform.Find("Shockwave");
		shockwaveVFX.localScale = Vector3.one * shockwaveRadius;

		searchAllyState = GetComponent<SearchAllyState_AI3>();
		fleeingState = GetComponent<FleeFromPlayerState_AI3>();
		superHealingState = GetComponent<SuperHealingState_AI3>();

		base.Awake();
	}

	private void Update()
	{
		if(attackStarted)
		{
			if (meleeAttack.Attack(transform.position, shockwaveRadius, true, false))
			{
				StartCoroutine(ShowAttackVFX());
			}
		}
	}

	IEnumerator ShowAttackVFX()
	{
		shockwaveVFX.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		shockwaveVFX.gameObject.SetActive(false);
	}

	public override AIState CheckConditions()
	{
		if (superHealingIsHappening)
		{
			return superHealingState;
		}

		if(attackStarted && Time.time > attackStartTime + attackDuration)
		{
			return fleeingState;
		}
		else if(!attackStarted && Vector3.Distance(transform.position, player.position) < maxStoppingDistFromPlayer)
		{
			attackStarted = true;
			attackStartTime = Time.time;
		}

		if (CanSeePoint(player.position + playerHeight * Vector3.up, nodeDist))
		{
			playerLastSeenTime = Time.time;
		}
		else if (Time.time >= playerLastSeenTime + timeBeforePlayerIsLost)
		{
			return searchAllyState;
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
		playerLastSeenTime = Time.time;
		attackStarted = false;
		superHealingIsHappening = false;
		SacrificeState_A2.SuperHealing += PursueSuperHealing;
		if (chasePlayer != null)
		{
			chasePlayer.Init(player, playerHeight, false, minStoppingDistFromPlayer, maxStoppingDistFromPlayer, true);
			chasePlayer.enabled = true;
		}
	}

	protected override void StateDidBecomeInactive(AIState nextState)
	{
		SacrificeState_A2.SuperHealing -= PursueSuperHealing;
		if (chasePlayer != null)
		{
			chasePlayer.enabled = false;
		}
	}
}
