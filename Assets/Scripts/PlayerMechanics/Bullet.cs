using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : DamagingObject
{
	public float bulletSpeed;

	protected override void Awake()
	{
		base.Awake();
		rb.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
	}
}
