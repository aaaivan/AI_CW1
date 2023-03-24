using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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

	private void OnCollisionEnter(Collision collision)
	{
		DamageableObject other = collision.gameObject.GetComponent<DamageableObject>();
		if (other != null)
		{
			other.TakeDamage(this);
		}
		Destroy(gameObject);
	}
}
