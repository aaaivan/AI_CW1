using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
	[SerializeField] protected string stateName;
	[SerializeField] protected float movementSpeed;
	[SerializeField] protected LayerMask layersBlockingView;
	[SerializeField] protected float sightDistance;
	[SerializeField][Range(0, 90.0f)] protected float fieldOfViewDeg;

	protected Transform bulletOrigin;
	protected Dictionary<string, AIState> adjacentStates = new Dictionary<string, AIState> ();
	protected Transform player;

	protected virtual void Awake()
	{
		bulletOrigin = transform.Find("BulletSpawnPos");
		player = GameManager.Instance.Player.transform;
	}

	public virtual AIState CheckConditions()
	{
		return null;
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
