using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
	[SerializeField] int maxHealth;
	int currentHealth;

	public int CurrentHealth { get { return currentHealth; } }

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
}
