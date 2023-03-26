using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : MonoBehaviour
{
	[SerializeField] int minDamage;
	[SerializeField] int maxDamage;
	[SerializeField] int criticalDamage;
	int damage;


	public int Damage { get { return damage; } }

	protected void Awake()
	{
		damage = minDamage; // TODO: more interesting function for damage
	}

	private void OnTriggerEnter(Collider other)
	{
		DamageableObject hitObj = other.gameObject.GetComponent<DamageableObject>();
		if (hitObj != null)
		{
			hitObj.TakeDamage(this);
		}
		Destroy(gameObject);
	}
}
