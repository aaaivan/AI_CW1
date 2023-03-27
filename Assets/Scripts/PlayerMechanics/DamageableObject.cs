using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
	[SerializeField] int maxHealth;
	int currentHealth;

	public int CurrentHealth { get { return currentHealth; } }
	public int MaxHealth { get { return maxHealth; } }
	public float CurrentHealthPercent { get { return (float)currentHealth / maxHealth; } }

	private void Awake()
	{
		currentHealth = maxHealth;
	}

	public void TakeDamage(DamagingObject obj)
	{
		TakeDamage(obj.Damage);
	}

	public void TakeDamage(int damage)
	{
		currentHealth -= damage;
		if (currentHealth <= 0)
		{
			GetComponent<IObjectCleanUp>()?.CleanUpObject();
		}
	}

	public void Heal(int healAmount)
	{
		currentHealth += healAmount;
		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
	}
}
