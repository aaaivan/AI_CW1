using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DamagingObject))]
public class BulletMovement : MonoBehaviour
{
	[SerializeField] float bulletSpeed;
	Rigidbody rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
	}
}
