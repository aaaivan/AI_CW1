using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SacrificeState_A2 : AIState
{
	[SerializeField] float healingRadius = 20.0f;
	[SerializeField] float maxTimeBeforeDying = 10.0f;
	[SerializeField] float healingPercentage = 0.4f;
	float countdownStartTime = 0;

	MoveToLocation moveToLocation;
	DamageableObject health;
	Transform particleVFX;
	Transform shield;

	public delegate void SuperHealingHandler(Transform healer);
	public static SuperHealingHandler SuperHealing;

	public float HealingRadius { get { return healingRadius; } }

	protected override void Awake()
	{
		moveToLocation = GetComponent<MoveToLocation>();
		health = GetComponent<DamageableObject>();
		particleVFX = transform.Find("Particles");
		shield = transform.Find("Shield");

		base.Awake();
	}

	private void Update()
	{
		if (Time.time > countdownStartTime + maxTimeBeforeDying)
		{
			foreach(var enemyType in EnemiesManager.Instance.GetEnemies())
			{
				foreach(var enemy in enemyType.Value)
				{
					if(Vector3.Distance(enemy.position, transform.position) < healingRadius)
					{
						DamageableObject h = enemy.GetComponent<DamageableObject>();
						h.Heal((int)(h.MaxHealth * healingPercentage) + health.CurrentHealth);
					}
				}
			}
			health.TakeDamage(health.CurrentHealth);
		}
	}

	protected override void StateDidBecomeActive(AIState prevState)
	{
		countdownStartTime = Time.time;
		particleVFX.gameObject.SetActive(true);
		shield.gameObject.SetActive(true);
		if(moveToLocation != null)
		{
			moveToLocation.enabled = true;
			// move to a location that is accessible to every character (i.e. not on mountains)
			PathfinderAgent playerPathfinder = player.GetComponent<PathfinderAgent>();
			moveToLocation.MoveTo(playerPathfinder.ClosestAccessibleLocation(transform.position));
		}
		if (SuperHealing != null)
		{
			SuperHealing.Invoke(transform);
		}
	}
}
