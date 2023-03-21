using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DamagingObject : MonoBehaviour
{
	protected Rigidbody rb;

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
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
