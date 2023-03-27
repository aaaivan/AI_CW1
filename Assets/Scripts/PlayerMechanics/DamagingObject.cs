using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : MonoBehaviour
{
	int damage = 0;
	public int Damage { get { return damage; } set { damage = value; } }

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
