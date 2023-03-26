using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
	[SerializeField] protected string stateName;

	protected LayerMask layersBlockingView;
	protected float sightDistance = 60;

	protected Transform bulletOrigin;
	protected Transform player;
	protected float playerHeight;
	protected bool isActive;

	protected float nodeDist;

	protected virtual void Awake()
	{
		layersBlockingView = LayerMask.GetMask(new string[] { "Default", "Terrain", "Unwalkable" });
		bulletOrigin = transform.Find("BulletSpawnPos");
		player = GameManager.Instance.Player.transform;
		playerHeight = player.GetComponent<CharacterController>().height;
		nodeDist = MapGenerator.Instance.terrainData.uniformScale;
	}

	public virtual AIState CheckConditions()
	{
		return null;
	}

	protected virtual void StateDidBecomeActive()
	{
		return;
	}

	protected virtual void StateDidBecomeInactive()
	{
		return;
	}


	public bool IsActive
	{
		set
		{
			if (isActive != value)
			{
				isActive = value;
				if (isActive)
				{
					this.enabled = true;
					StateDidBecomeActive();
				}
				else
				{
					this.enabled = false;
					StateDidBecomeInactive();
				}
			}
		}
	}

	public string StateName { get { return stateName; } }

	protected bool CanSeePoint(Vector3 point, float radius)
	{
		Vector3 bulletToPointDirection = point - bulletOrigin.position;

		// player is too far
		if (Vector3.Distance(transform.position, point) > sightDistance)
			return false;

		// view blocked by terrain
		if (Physics.Raycast(bulletOrigin.position, bulletToPointDirection.normalized,
			bulletToPointDirection.magnitude - radius, layersBlockingView, QueryTriggerInteraction.Ignore))
		{
			return false;
		}

		// good to go
		return true;
	}

}
