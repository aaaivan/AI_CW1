using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeAttack : MonoBehaviour
{
	[SerializeField] float meleeAttackCooldownTime = 1f;
	float lastMeleeAttackTime = 0;

	IMeleeDamageCalculator meleeDamage;
	private void Awake()
	{
		meleeDamage = GetComponent<IMeleeDamageCalculator>();
	}

	public bool Attack(Vector3 center, float radius, bool damagePlayer, bool damageEnemies)
	{
		if (meleeDamage == null || Time.time < lastMeleeAttackTime + meleeAttackCooldownTime)
		{
			return false;
		}
		lastMeleeAttackTime = Time.time;

		int damage = meleeDamage.GetMeleeDamage();
		Collider[] coliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask(new string[] { "Character" }), QueryTriggerInteraction.Ignore);
		HashSet<DamageableObject> targets = new HashSet<DamageableObject>();
		foreach (var col in coliders)
		{
			GameObject go = col.gameObject;
			if ((damagePlayer && go.tag == "Player")
			|| (damageEnemies && go.tag != "Player"))
			{
				DamageableObject damageableObject = go.GetComponent<DamageableObject>();
				if (go != null)
				{
					targets.Add(damageableObject);

				}
			}
		}
		foreach (var t in targets)
		{
			t.TakeDamage(damage);
		}

		return true;
	}
}
