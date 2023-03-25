using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
	[SerializeField] protected string stateName;
	[SerializeField] protected LayerMask layersBlockingView;
	[SerializeField] protected float sightDistance;
	[SerializeField][Range(0, 90.0f)] protected float fieldOfViewDeg;

	protected Transform bulletOrigin;
	protected Dictionary<string, AIState> adjacentStates = new Dictionary<string, AIState> ();
	protected Transform player;
	protected bool isActive;

	protected float nodeDist;

	protected virtual void Awake()
	{
		bulletOrigin = transform.Find("BulletSpawnPos");
		player = GameManager.Instance.Player.transform;
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
					StateDidBecomeActive();
				}
				else
				{
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

		// player is out of the field of view
		if (Vector3.Angle(transform.forward, bulletToPointDirection) > fieldOfViewDeg)
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
