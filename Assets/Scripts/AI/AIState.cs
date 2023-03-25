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

	protected Transform sightOrigin;
	protected Dictionary<string, AIState> adjacentStates = new Dictionary<string, AIState> ();
	protected Transform player;

	protected virtual void Awake()
	{
		sightOrigin = transform.Find("Sight");
		player = GameManager.Instance.Player.transform;
	}

	public virtual AIState CheckConditions()
	{
		return null;
	}

	public string StateName { get { return stateName; } }

	protected bool CanSeePoint(Vector3 point)
	{
		Vector3 enemyToPointDirection = point - transform.position;

		// player is too far
		if (Vector3.Distance(transform.position, point) > sightDistance)
			return false;

		// player is out of the field of view
		if (Vector3.Angle(transform.forward, enemyToPointDirection) > fieldOfViewDeg)
			return false;

		// view blocked by terrain
		if (Physics.Raycast(sightOrigin.position, enemyToPointDirection.normalized, sightDistance, layersBlockingView.value))
			return false;

		// good to go
		return true;
	}

}
