using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathfinderUtils
{
	public static List<Vector3> RetracePath(PathfinderNode startNode, PathfinderNode targetNode, bool simplifyPath)
	{
		List<PathfinderNode> nodes = new List<PathfinderNode>();
		List<Vector3> path = new List<Vector3>();
		PathfinderNode node = targetNode;
		while (node != startNode && node != null)
		{
			path.Add(node.position);
			nodes.Add(node);
			node = node.parent;
		}
		if (node != null)
		{
			path.Add(node.position);
			nodes.Add(node);
		}
		if(simplifyPath)
		{
			path = SimplifyPath(nodes);
		}
		path.Reverse();
		return path;
	}

	static List<Vector3> SimplifyPath(List<PathfinderNode> nodes)
	{
		List<Vector3> simplifiedPath = new List<Vector3>();
		Vector2 currentDir = Vector2.zero;
		for(int i = 0; i < nodes.Count - 1; i++)
		{
			Vector2 dir = new Vector2(nodes[i + 1].x - nodes[i].x,
									nodes[i + 1].y - nodes[i].y);
			if(dir != currentDir)
			{
				simplifiedPath.Add(nodes[i].position);
				currentDir = dir;
			}
		}
		simplifiedPath.Add(nodes[nodes.Count - 1].position);
		return simplifiedPath;
	}

	public static int GetDistance(PathfinderNode fromNode, PathfinderNode toNode)
	{
		int dx = Mathf.Abs(fromNode.x - toNode.x);
		int dy = Mathf.Abs(fromNode.y - toNode.y);
		int min = Mathf.Min(dx, dy);
		int max = Mathf.Max(dx, dy);

		int dist = 14 * min + 10 * (max - min);
		return dist;
	}
}
