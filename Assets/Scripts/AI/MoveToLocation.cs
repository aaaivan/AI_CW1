using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToLocation : MonoBehaviour
{
	float stoppingDistance;

	CharacterMovement characterMovement;
	PathfinderAgent pathfinderAgent;
	Vector3 targetLocation;
	List<Vector3> path;
	int currentWaypointIndex = 0;

	private void Awake()
	{
		characterMovement = GetComponent<CharacterMovement>();
		pathfinderAgent = GetComponent<PathfinderAgent>();
		stoppingDistance = GetComponent<CharacterController>().radius;
	}

	public void MoveTo(Vector3 target)
	{
		targetLocation = pathfinderAgent.ClosestAccessibleLocation(target);
		SubmitNewPathRequest();
	}

	void SubmitNewPathRequest()
	{
		PathRequestManager.Instance.RequestPath(transform.position, targetLocation, OnNewPathReceived, pathfinderAgent, true);
	}

	void OnNewPathReceived(List<Vector3> path)
	{
		if (path.Count > 0)
		{
			this.path = path;
			StartCoroutine(MovementCoroutine());
		}
		else
		{
			// we might have gotten stuck in a corner
			// walk towards the closes accessible node
			Vector3 pos = pathfinderAgent.ClosestAccessibleLocation(transform.position);
			characterMovement.MoveTowards(pos);
			SubmitNewPathRequest();
		}
	}

	IEnumerator MovementCoroutine()
	{
		currentWaypointIndex = 0;
		while (currentWaypointIndex < path.Count)
		{
			characterMovement.MoveTowards(path[currentWaypointIndex]);
			yield return null;
			if (Vector3.Distance(transform.position, path[currentWaypointIndex]) < stoppingDistance)
			{
				currentWaypointIndex++;
			}
		}
		path = null;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		path = null;
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		path = null;
	}

	private void OnDrawGizmosSelected()
	{
		if (path != null && Application.isPlaying)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, path[currentWaypointIndex]);
			for (int i = currentWaypointIndex; i < path.Count; ++i)
			{
				Gizmos.DrawSphere(path[i], MapGenerator.Instance.terrainData.uniformScale / 2);
				if (i < path.Count - 1)
				{
					Gizmos.DrawLine(path[i], path[i + 1]);
				}
			}
		}
	}
}
