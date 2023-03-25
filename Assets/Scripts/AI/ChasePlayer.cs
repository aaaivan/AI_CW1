using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour
{
	[SerializeField] float stoppingDistanceFromPlayer = 10;
	[SerializeField] LayerMask layersBlockingView;
	float stoppingDistance;
	Transform bulletOrigin;
	Transform player;
	const float pathUpdateTimeInterval = 0.5f;
	float lastPathUpdateTime = 0f;

	CharacterMovement characterMovement;
	PathfinderAgent pathfinderAgent;
	bool waitingForPath = false;

	List<Vector3> currentPath;
	int currentWaypointIndex;

	private void Awake()
	{
		bulletOrigin = transform.Find("BulletSpawnPos");
		player = GameManager.Instance.Player.transform;
		characterMovement = GetComponent<CharacterMovement>();
		pathfinderAgent = GetComponent<PathfinderAgent>();
	}
	private void Start()
	{
		stoppingDistance = pathfinderAgent.NodeDist;
	}

	private void Update()
	{
		if(currentPath == null && !waitingForPath)
		{
			SubmitNewPathRequest();
		}
		else if(!waitingForPath && Time.time >= lastPathUpdateTime + pathUpdateTimeInterval)
		{
			SubmitNewPathRequest();
		}
	}

	void SubmitNewPathRequest()
	{
		lastPathUpdateTime = Time.time;
		waitingForPath = true;
		PathRequestManager.Instance.RequestPath(transform.position, player.transform.position, OnNewPathReceived, pathfinderAgent, true);
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
		for(int i = 0; i < newPath.Count; i++)
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
			float scaledStoppingDist = stoppingDistanceFromPlayer * pathfinderAgent.NodeDist;
			bool canMove = Vector3.Distance(transform.position, player.transform.position) > scaledStoppingDist;
			if(!canMove)
			{
				// if we are closer than the stopping distance but the player
				// is hidden behind the terrain chase it anyway
				RaycastHit hit;
				Vector3 rayDirection = player.position - bulletOrigin.position;
				if(Physics.Raycast(bulletOrigin.position, rayDirection.normalized, out hit, rayDirection.magnitude, layersBlockingView, QueryTriggerInteraction.Ignore))
				{
					if(hit.collider.gameObject.tag != "Player")
					{
						canMove = true;
					}
				}
			}
			if(canMove)
			{
				characterMovement.MoveTowards(currentPath[currentWaypointIndex]);
			}
			characterMovement.RotateTowards(player.transform.position);
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
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		currentPath = null;
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
