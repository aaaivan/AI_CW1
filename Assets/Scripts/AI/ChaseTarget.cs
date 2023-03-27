using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTarget : MonoBehaviour
{
	[SerializeField] LayerMask layersBlockingView;

	List<Vector3> currentPath;
	int currentWaypointIndex;
	bool waitingForPath = false;
	const float pathUpdateTimeInterval = 0.5f;
	float lastPathUpdateTime = 0f;
	float stoppingDistanceFromWaypoint;
	float stoppingDistanceFromTarget = 10;
	bool shootWhileChasing = false;

	CharacterMovement characterMovement;
	Shooting shootingController;
	PathfinderAgent pathfinderAgent;
	Transform bulletOrigin;
	Transform chaseTarget;
	float chaseTargetHeight;

	public bool ShootWhileChasing
	{
		get {  return shootWhileChasing; }
		set {  shootWhileChasing = value; }
	}

	public Transform Target { get { return chaseTarget; } }

	private void Awake()
	{
		bulletOrigin = transform.Find("BulletSpawnPos");
		characterMovement = GetComponent<CharacterMovement>();
		shootingController = GetComponent<Shooting>();
		pathfinderAgent = GetComponent<PathfinderAgent>();
		stoppingDistanceFromWaypoint = GetComponent<CharacterController>().radius;
	}

	public void Init(Transform _chaseTarget, float _chaseTargetHeight, bool _shoot, float _stoppingDistanceFromTarget)
	{
		chaseTarget = _chaseTarget;
		chaseTargetHeight = _chaseTargetHeight;
		shootWhileChasing = _shoot;
		stoppingDistanceFromTarget = _stoppingDistanceFromTarget;
	}

	private void Update()
	{
		if (chaseTarget == null)
			return;

		if(currentPath == null && !waitingForPath)
		{
			SubmitNewPathRequest();
		}
		else if(!waitingForPath && Time.time >= lastPathUpdateTime + pathUpdateTimeInterval)
		{
			SubmitNewPathRequest();
		}

		if(shootWhileChasing)
		{
			Vector3 shootDir = chaseTarget.transform.position
				+ Vector3.up * chaseTargetHeight/ 2
				- bulletOrigin.position;
			shootingController.Shoot(shootDir.normalized);
		}
	}

	void SubmitNewPathRequest()
	{
		lastPathUpdateTime = Time.time;
		waitingForPath = true;
		PathRequestManager.Instance.RequestPath(transform.position, pathfinderAgent.ClosestAccessibleLocation(chaseTarget.transform.position), OnNewPathReceived, pathfinderAgent, true);
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
			if (chaseTarget == null)
				break;

			float scaledStoppingDist = stoppingDistanceFromTarget * pathfinderAgent.NodeDist;
			bool canMove = Vector3.Distance(transform.position, chaseTarget.transform.position) > scaledStoppingDist;
			if(!canMove)
			{
				// if we are closer than the stopping distance but the player
				// is hidden behind the terrain chase it anyway
				RaycastHit hit;
				Vector3 rayDirection = chaseTarget.position - bulletOrigin.position;
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
			characterMovement.RotateTowards(chaseTarget.transform.position);
			yield return null;
			if (Vector3.Distance(transform.position, currentPath[currentWaypointIndex]) < stoppingDistanceFromWaypoint)
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
