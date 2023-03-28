using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeFromPlayer : MonoBehaviour
{
	List<Vector3> currentPath;
	int currentWaypointIndex;
	bool waitingForPath = false;

	CharacterMovement characterMovement;
	PathfinderAgent pathfinderAgent;
	Transform player;
	float stoppingDistance;

	private void Awake()
	{
		player = GameManager.Instance.Player.transform;
		characterMovement = GetComponent<CharacterMovement>();
		pathfinderAgent = GetComponent<PathfinderAgent>();
		stoppingDistance = GetComponent<CharacterController>().radius;
	}

	private void Update()
	{
		if (currentPath == null && !waitingForPath)
		{
			SubmitNewPathRequest();
		}
	}

	void SubmitNewPathRequest()
	{
		if (player == null)
			return;

		waitingForPath = true;

		Vector3 fleeDirection = transform.position - player.transform.position;
		float blending = fleeDirection.magnitude / Mathf.Min(MapGenerator.Instance.MapInnerRect.width, MapGenerator.Instance.MapInnerRect.height);
		Mathf.Clamp01(blending);
		fleeDirection.Normalize();
		fleeDirection = fleeDirection * (1 - blending) + transform.forward * blending;
		fleeDirection.Normalize();
		PathRequestManager.Instance.RequestPath(transform.position, RandomLocation(transform.position, fleeDirection), OnNewPathReceived, pathfinderAgent, true);
	}

	Vector3 RandomLocation(Vector3 pivot, Vector3 forward)
	{
		forward.y = 0;
		Rect mapRect = MapGenerator.Instance.MapInnerRect;
		float angle = 120 * (Random.value - 0.5f);
		float dist = Mathf.Min(mapRect.width, mapRect.height) * (Random.value / 4 + 0.25f);

		Vector3 deltaPos = Quaternion.Euler(0, angle, 0) * forward.normalized;
		deltaPos *= dist;

		Vector3 newPosition = pivot + deltaPos;

		for (int i = 0; i < 2; ++i)
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
				deltaPos = newPosition - pivot;
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
		if (path.Count > 0)
		{
			BlendToNewPath(path);
			StopAllCoroutines();
			StartCoroutine(MovementCoroutine());
		}
		else
		{
			// we might have gotten stuck in a corner
			// walk towards the closes accessible node
			Vector3 pos = pathfinderAgent.ClosestAccessibleLocation(transform.position);
			characterMovement.MoveTowards(pos);
		}
	}

	void BlendToNewPath(List<Vector3> newPath)
	{
		float minDist = float.MaxValue;
		int minDistIndex = 0;
		// find waypoint closest to player
		for (int i = 0; i < newPath.Count; i++)
		{
			float dist = Vector3.Distance(transform.position, newPath[i]);
			if (dist < minDist)
			{
				minDist = dist;
				minDistIndex = i;
			}
		}
		if (minDistIndex < newPath.Count - 1)
		{
			Vector3 d1 = newPath[minDistIndex] - transform.position;
			Vector3 d2 = newPath[minDistIndex + 1] - newPath[minDistIndex];
			float angle = Vector3.Angle(d1, d2);
			if (angle < 90)
			{
				minDistIndex += 1;
			}
		}
		// join out current location and the closest point in the new path
		currentPath = pathfinderAgent.FindPathToLocation(newPath[minDistIndex], true);
		newPath.RemoveRange(0, minDistIndex + 1);
		currentPath.AddRange(newPath);
	}

	IEnumerator MovementCoroutine()
	{
		currentWaypointIndex = 0;
		while (currentWaypointIndex < currentPath.Count)
		{
			characterMovement.MoveTowards(currentPath[currentWaypointIndex]);
			yield return null;
			if (Vector3.Distance(transform.position, currentPath[currentWaypointIndex]) < stoppingDistance)
			{
				currentWaypointIndex++;
				if (currentWaypointIndex == currentPath.Count - 1 && !waitingForPath) // last waypoint
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
		waitingForPath = false;
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		currentPath = null;
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
