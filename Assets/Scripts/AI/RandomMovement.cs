using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RandomMovement : MonoBehaviour
{
	float stoppingDistance;

	CharacterMovement characterMovement;
	PathfinderAgent pathfinderAgent;

	List<Vector3> currentPath;
	int currentWaypointIndex;
	List<Vector3> nextPath;
	bool waitingForPath = false;

	private void Awake()
	{
		characterMovement = GetComponent<CharacterMovement>();
		pathfinderAgent = GetComponent<PathfinderAgent>();
	}

	private void Start()
	{
		stoppingDistance = pathfinderAgent.NodeDist;
	}

	private void Update()
	{
		if (nextPath == null && !waitingForPath)
		{
			SubmitNewPathRequest();
		}
		if (currentPath == null && nextPath != null)
		{
			currentPath = nextPath;
			nextPath = null;
			StopAllCoroutines();
			StartCoroutine(MovementCoroutine());
		}
	}

	void SubmitNewPathRequest()
	{
		Vector3 pivot;
		Vector3 forward;
		if (currentPath != null && currentPath.Count > 0)
		{
			pivot = currentPath[currentPath.Count - 1];
			if (currentPath.Count > 1)
			{
				forward = currentPath[currentPath.Count - 1] - currentPath[currentPath.Count - 1];
			}
			else
			{
				forward = transform.forward;
			}
		}
		else
		{
			pivot = transform.position;
			forward = transform.forward;
		}
		PathRequestManager.Instance.RequestPath(pivot, RandomLocation(pivot, forward), OnNewPathReceived, pathfinderAgent);
		waitingForPath = true;
	}

	Vector3 RandomLocation(Vector3 pivot, Vector3 forward)
	{
		Rect mapRect = MapGenerator.Instance.MapInnerRect;
		float angle = 120 * (Random.value - 0.5f);
		float dist = Mathf.Min(mapRect.width, mapRect.height) * (Random.value/4 + 0.25f);

		Vector3 deltaPos = Quaternion.Euler(0, angle, 0) * forward.normalized;
		deltaPos *= dist;

		Vector3 newPosition = pivot + deltaPos;
		// make sure new point is within the map bounds
		if(newPosition.x < mapRect.x ||
			newPosition.x > mapRect.x + mapRect.width)
		{
			deltaPos.x = -deltaPos.x;
			newPosition = pivot + deltaPos;
		}
		if (newPosition.y < mapRect.y ||
			newPosition.y > mapRect.y + mapRect.height)
		{
			deltaPos.y = -deltaPos.y;
			newPosition = pivot + deltaPos;
		}
		return pathfinderAgent.ClosestAccessibleLocation(newPosition);
	}

	void OnNewPathReceived(List<Vector3> path)
	{
		waitingForPath = false;
		nextPath = path;
	}

	IEnumerator MovementCoroutine()
	{
		currentWaypointIndex = 0;
		while(currentWaypointIndex < currentPath.Count)
		{
			characterMovement.MoveTowards(currentPath[currentWaypointIndex]);
			if(Vector3.Distance(transform.position, currentPath[currentWaypointIndex]) < stoppingDistance)
			{
				currentWaypointIndex++;
				if(currentWaypointIndex >= currentPath.Count && nextPath != null)
				{
					currentPath = nextPath;
					currentWaypointIndex = 0;
					nextPath = null;
				}
				else if(currentWaypointIndex == currentPath.Count - 1
					&& nextPath == null && !waitingForPath) // last waypoint
				{
					SubmitNewPathRequest();
				}
			}
			yield return null;
		}
		currentPath = null;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		currentPath = null;
		nextPath = null;
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		currentPath = null;
		nextPath = null;
	}
}
