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
		stoppingDistance = GetComponent<CharacterController>().radius;
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
		waitingForPath = true;
		if (currentPath != null && currentPath.Count > 0)
		{
			pivot = currentPath[currentPath.Count - 1];
			if (currentPath.Count > 1)
			{
				forward = currentPath[currentPath.Count - 1] - currentPath[currentPath.Count - 2];
			}
			else
			{
				forward = currentPath[currentPath.Count - 1] - transform.position;
			}
		}
		else
		{
			pivot = transform.position;
			forward = transform.forward;
		}
		PathRequestManager.Instance.RequestPath(pivot, RandomLocation(pivot, forward), OnNewPathReceived, pathfinderAgent, true);
	}

	Vector3 RandomLocation(Vector3 pivot, Vector3 forward)
	{
		forward.y = 0;
		Rect mapRect = MapGenerator.Instance.MapInnerRect;
		float angle = 120 * (Random.value - 0.5f);
		float dist = Mathf.Min(mapRect.width, mapRect.height) * (Random.value/4 + 0.25f);

		Vector3 deltaPos = Quaternion.Euler(0, angle, 0) * forward.normalized;
		deltaPos *= dist;

		Vector3 newPosition = pivot + deltaPos;

		for(int i = 0; i < 2; ++i)
		{
			// make sure new point is within the map bounds
			if (newPosition.x < mapRect.x)
			{
				float overshootAmount = mapRect.x - newPosition.x;
				Vector3 v1 = deltaPos * (1 - overshootAmount / Mathf.Abs(deltaPos.x));
				Vector3 v2 = deltaPos * (overshootAmount / Mathf.Abs(deltaPos.x));
				float sign = Mathf.Sign(Vector3.Angle(v1, Vector3.back) - 90);
				if (sign > 0)
				{
					v2 = Quaternion.Euler(0, 90, 0) * v2;
				}
				else
				{
					v2 = Quaternion.Euler(0, -90, 0) * v2;
				}
				newPosition = pivot + v1 + v2;
				deltaPos = newPosition - pivot;
			}
			else if (newPosition.x > mapRect.x + mapRect.width)
			{
				float overshootAmount = newPosition.x - mapRect.x - mapRect.width;
				Vector3 v1 = deltaPos * (1 - overshootAmount / Mathf.Abs(deltaPos.x));
				Vector3 v2 = deltaPos * (overshootAmount / Mathf.Abs(deltaPos.x));
				float sign = Mathf.Sign(Vector3.Angle(v1, Vector3.forward) - 90);
				if (sign > 0)
				{
					v2 = Quaternion.Euler(0, 90, 0) * v2;
				}
				else
				{
					v2 = Quaternion.Euler(0, -90, 0) * v2;
				}
				newPosition = pivot + v1 + v2;
				deltaPos = newPosition - pivot;;
			}

			if (newPosition.z < mapRect.y)
			{
				float overshootAmount = mapRect.y - newPosition.z;
				Vector3 v1 = deltaPos * (1 - overshootAmount / Mathf.Abs(deltaPos.z));
				Vector3 v2 = deltaPos * (overshootAmount / Mathf.Abs(deltaPos.z));
				float sign = Mathf.Sign(Vector3.Angle(v1, Vector3.right) - 90);
				if (sign > 0)
				{
					v2 = Quaternion.Euler(0, 90, 0) * v2;
				}
				else
				{
					v2 = Quaternion.Euler(0, -90, 0) * v2;
				}
				newPosition = pivot + v1 + v2;
				deltaPos = newPosition - pivot;
			}
			else if (newPosition.z > mapRect.y + mapRect.height)
			{
				float overshootAmount = newPosition.z - mapRect.y - mapRect.height;
				Vector3 v1 = deltaPos * (1 - overshootAmount / Mathf.Abs(deltaPos.z));
				Vector3 v2 = deltaPos * (overshootAmount / Mathf.Abs(deltaPos.z));
				float sign = Mathf.Sign(Vector3.Angle(v1, Vector3.left) - 90);
				if (sign > 0)
				{
					v2 = Quaternion.Euler(0, 90, 0) * v2;
				}
				else
				{
					v2 = Quaternion.Euler(0, -90, 0) * v2;
				}
				newPosition = pivot + v1 + v2;
				deltaPos = newPosition - pivot;
			}
		}

		return pathfinderAgent.ClosestAccessibleLocation(newPosition);
	}

	void OnNewPathReceived(List<Vector3> path)
	{
		waitingForPath = false;
		if(path.Count > 0)
		{
			nextPath = path;
		}
		else
		{
			// we might have gotten stuck in a corner
			// walk towards the closes accessible node
			Vector3 pos = pathfinderAgent.ClosestAccessibleLocation(transform.position);
			characterMovement.MoveTowards(pos);
		}
	}

	IEnumerator MovementCoroutine()
	{
		currentWaypointIndex = 0;
		while(currentWaypointIndex < currentPath.Count)
		{
			characterMovement.MoveTowards(currentPath[currentWaypointIndex]);
			yield return null;
			if (Vector3.Distance(transform.position, currentPath[currentWaypointIndex]) < stoppingDistance)
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
		}
		currentPath = null;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		currentPath = null;
		nextPath = null;
		waitingForPath = false;
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		currentPath = null;
		nextPath = null;
		waitingForPath = false;
	}

	private void OnDrawGizmosSelected()
	{
		if (currentPath != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, currentPath[currentWaypointIndex]);
			for (int i = currentWaypointIndex; i < currentPath.Count; ++i)
			{
				Gizmos.DrawSphere(currentPath[i], MapGenerator.Instance.terrainData.uniformScale / 2);
				if (i < currentPath.Count - 1)
				{
					Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
				}
			}
		}
	}
}
