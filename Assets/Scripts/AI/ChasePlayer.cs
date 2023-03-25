using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour
{
	[SerializeField] float stoppingDistanceFromPlayer = 10;
	[SerializeField] LayerMask layersBlockingView;
	float stoppingDistance;
	Transform sightOrigin;
	Transform player;
	const float pathUpdateTimeInterval = 0.5f;
	float lastPathUpdateTime = 0f;

	CharacterMovement characterMovement;
	PathfinderAgent pathfinderAgent;
	bool waitingForPath = false;

	List<Vector3> currentPath;
	int currentWaypointIndex;
	List<Vector3> nextPath;

	private void Awake()
	{
		sightOrigin = transform.Find("Sight");
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
		Vector3 start = transform.position;
		if (currentPath != null && currentWaypointIndex < currentPath.Count)
		{
			start = currentPath[currentWaypointIndex];
		}
		PathRequestManager.Instance.RequestPath(start, player.transform.position, OnNewPathReceived, pathfinderAgent);
	}

	void OnNewPathReceived(List<Vector3> path)
	{
		waitingForPath = false;
		if (path.Count > 0)
		{
			nextPath = path;
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

	IEnumerator MovementCoroutine()
	{
		currentWaypointIndex = 0;
		while (currentWaypointIndex < currentPath.Count)
		{
			float scaledStoppingDist = stoppingDistanceFromPlayer * pathfinderAgent.NodeDist;
			bool canMove = Vector3.Distance(transform.position, player.transform.position) > scaledStoppingDist;
			if(!canMove)
			{
				// if we are closer than the stopping distance but the player is hidden behind the terrain
				// chase it anyway
				canMove = Physics.Raycast(sightOrigin.position, transform.forward, scaledStoppingDist, layersBlockingView.value);
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
