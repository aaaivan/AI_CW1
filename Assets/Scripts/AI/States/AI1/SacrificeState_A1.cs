using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeState_A1 : AIState
{
	[SerializeField] float stoppingDistanceFromPlayer = 2.0f;
	[SerializeField] float explosionRadius = 5.0f;
	[SerializeField] float maxTimeBeforeExploding = 10.0f;
	float countdownStartTime = 0;

	ChaseTarget chasePlayer;
	DamageableObject health;
	Transform explosionVFX;

	protected override void Awake()
	{
		chasePlayer = GetComponent<ChaseTarget>();
		health = GetComponent<DamageableObject>();
		explosionVFX = transform.Find("Explosion");

		base.Awake();
	}

	private void Update()
	{
		if(Time.time > countdownStartTime + maxTimeBeforeExploding
			|| Vector3.Distance(transform.position, player.transform.position) <= stoppingDistanceFromPlayer)
		{
			GetComponent<Shooting>().MeleeAttack(transform.position, explosionRadius, true, false);
			health.TakeDamage(health.CurrentHealth);
		}
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		countdownStartTime = Time.time;
		if (chasePlayer != null)
		{
			chasePlayer.Init(player, playerHeight, false, stoppingDistanceFromPlayer);
			chasePlayer.enabled = true;
			explosionVFX.gameObject.SetActive(true);
		}
	}
}
