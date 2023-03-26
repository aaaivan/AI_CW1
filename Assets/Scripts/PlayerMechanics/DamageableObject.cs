using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
	[SerializeField] int maxHealth;
	int currentHealth;

	public int CurrentHealth { get { return currentHealth; } }
	public int MaxHealth { get { return maxHealth; } }
	public float HealthLeftPercent { get { return (float)currentHealth / maxHealth; } }

	private void Awake()
	{
		currentHealth = maxHealth;
	}

	public void TakeDamage(DamagingObject obj)
	{
		currentHealth -= obj.Damage;
		if(currentHealth <= 0)
		{
			Destroy(gameObject);
		}
	}

	public void Heal(int healAmount)
	{
		currentHealth += healAmount;
		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
	}
}
