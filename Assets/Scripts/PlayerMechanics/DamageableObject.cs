using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
	[SerializeField] int maxHealth;
	int currentHealth;

	IAttackMitigationCalculator attackMitigation;

	public int CurrentHealth { get { return currentHealth; } }
	public int MaxHealth { get { return maxHealth; } }
	public float CurrentHealthPercent { get { return (float)currentHealth / maxHealth; } }

	private void Awake()
	{
		currentHealth = maxHealth;
		attackMitigation = GetComponent<IAttackMitigationCalculator>();
	}

	public void TakeDamage(DamagingObject obj)
	{
		TakeDamage(obj.Damage);
	}

	public void TakeDamage(int damage)
	{
		int defence = attackMitigation.GetDefenceValue(damage);
		damage = Mathf.Clamp(damage - defence, 0, int.MaxValue);
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
