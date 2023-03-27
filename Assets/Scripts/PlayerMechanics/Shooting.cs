using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
	public GameObject bulletPrefab;
	public Transform bulletSpawnPosition;
	public float shootingCooldownTime = 0.5f;

	float lastShootTime = 0;

	IMeleeDamageCalculator meleeDamage;
	IBulletDamageCalculator bulletDamage;

	public float ShootingCooldownTime { get { return shootingCooldownTime; } }

	private void Awake()
	{
		meleeDamage = GetComponent<IMeleeDamageCalculator>();
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

	public void MeleeAttack(Vector3 center, float radius, bool damagePlayer, bool damageEnemies)
	{
		if(meleeDamage == null)
		{
			return;
		}

		int damage = meleeDamage.GetMeleeDamage();
		Collider[] coliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask(new string[] { "Character" }), QueryTriggerInteraction.Ignore);
		HashSet<DamageableObject> targets = new HashSet<DamageableObject>();
		foreach(var col in coliders)
		{
			GameObject go = col.gameObject;
			if(damagePlayer && go.tag == "Player"
			|| damageEnemies && go.tag!= "Player")
			{
				DamageableObject damageableObject = go.GetComponent<DamageableObject>();
				if (go != null)
				{
					targets.Add(damageableObject);

				}
			}
		}
		foreach(var t in targets)
		{
			t.TakeDamage(damage);
		}

	}
}
