using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
	public GameObject bulletPrefab;
	public Transform bulletSpawnPosition;
	public float shootingCooldownTime = 0.5f;

	float lastShootTime = 0;

	IBulletDamageCalculator bulletDamage;

	public float ShootingCooldownTime { get { return shootingCooldownTime; } }

	private void Awake()
	{
		bulletDamage = GetComponent<IBulletDamageCalculator>();
	}

	public void Shoot(Vector3 shootDirection)
	{
		if (Time.time < lastShootTime + shootingCooldownTime) return;

		lastShootTime = Time.time;
		GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.LookRotation(shootDirection.normalized));
		Collider bulletColl = bullet.GetComponent<Collider>();
		if(bulletColl != null )
		{
			foreach (var c in GetComponentsInChildren<Collider>())
			{
				Physics.IgnoreCollision(c, bulletColl, true);
			}
		}

		int damage = bulletDamage.GetBulletDamage();
		bullet.GetComponent<DamagingObject>().Damage = damage;
	}
}
