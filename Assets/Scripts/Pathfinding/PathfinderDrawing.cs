using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderDrawing : MonoBehaviour
{
	PathfinderAgent agent;

	private void Awake()
	{
		agent = GetComponent<PathfinderAgent>();
	}

	private void OnDrawGizmosSelected()
	{
		if (agent != null && GameManager.Instance != null)
		{
			Vector3 target = GameManager.Instance.Player.transform.position; // TODO: replace with current target
			List<Vector3> path = agent.FindPathToLocation(target);
			Gizmos.color = Color.blue;
			foreach (var n in path)
			{
				Gizmos.DrawSphere(n, MapGenerator.Instance.terrainData.uniformScale / 2);
			}
		}
	}
}
