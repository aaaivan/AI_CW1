using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
	[SerializeField] protected string stateName;

	protected LayerMask layersBlockingView;

	protected Transform eyes;
	protected Transform player;
	protected float playerHeight;
	protected bool isActive;
	protected float nodeDist;

	protected FiniteStateMachine fsm;

	protected virtual void Awake()
	{
		layersBlockingView = LayerMask.GetMask(new string[] { "Default", "Terrain", "Unwalkable" });
		eyes = transform.Find("Eyes");
		player = GameManager.Instance.Player.transform;
		playerHeight = player.GetComponent<CharacterController>().height;
		nodeDist = MapGenerator.Instance.terrainData.uniformScale;
		fsm = GetComponent<FiniteStateMachine>();
	}

	public virtual AIState CheckConditions()
	{
		return null;
	}

	protected virtual void StateDidBecomeActive(AIState prevState)
	{
		return;
	}

	protected virtual void StateDidBecomeInactive(AIState nextState)
	{
		return;
	}

	public void SetActive(AIState prevState)
	{
		if (!isActive)
		{
			isActive = true;
			this.enabled = true;
			StateDidBecomeActive(prevState);
		}
	}

	public void SetInactive(AIState nextState)
	{ 
		if (isActive)
		{
			isActive = false;
			this.enabled = false;
			StateDidBecomeInactive(nextState);
		}
	}

	public string StateName { get { return stateName; } }

	protected bool CanSeePoint(Vector3 point, float radius)
	{
		Vector3 bulletToPointDirection = point - eyes.position;

		// player is too far
		if (Vector3.Distance(transform.position, point) > fsm.SightDistance)
			return false;

		// view blocked by terrain
		if (Physics.Raycast(eyes.position, bulletToPointDirection.normalized,
			bulletToPointDirection.magnitude - radius, layersBlockingView, QueryTriggerInteraction.Ignore))
		{
			return false;
		}

		// good to go
		return true;
	}

}
