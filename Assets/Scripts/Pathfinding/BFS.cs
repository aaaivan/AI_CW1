using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BFS
{
	public static bool[] FindConnectedNodes(PathfinderNode entryNode, int totalNodes)
	{
		Queue<PathfinderNode> queue = new Queue<PathfinderNode>();
		bool[] visited = new bool[totalNodes];

		visited[entryNode.id] = true;
		queue.Enqueue(entryNode);

		while (queue.Count > 0)
		{
			PathfinderNode node = queue.Dequeue();
			foreach(PathfinderNode n in node.neighbours)
			{
				if (!visited[n.id] && node.walkable == n.walkable)
				{
					visited[n.id] = true;
					queue.Enqueue(n);
				}
			}
		}
		return visited;
	}

	public static PathfinderNode FindClosestAccessibleNode(PathfinderNode entryNode, int totalNodes)
	{
		if (entryNode.accessible && entryNode.walkable)
			return entryNode;

		Queue<PathfinderNode> queue = new Queue<PathfinderNode>();
		bool[] visited = new bool[totalNodes];

		visited[entryNode.id] = true;
		queue.Enqueue(entryNode);

		while (queue.Count > 0)
		{
			PathfinderNode node = queue.Dequeue();
			foreach (PathfinderNode n in node.neighbours)
			{
				if (!visited[n.id])
				{
					if(n.accessible && n.walkable)
					{
						return n;
					}
					visited[n.id] = true;
					queue.Enqueue(n);
				}
			}
		}

		return null;
	}
}
