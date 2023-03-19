using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BFS
{
	public static bool[] FindConnectedNodes(AStarNode entryNode, int totalNodes)
	{
		Queue<AStarNode> queue = new Queue<AStarNode>();
		bool[] visited = new bool[totalNodes];

		visited[entryNode.id] = true;
		queue.Enqueue(entryNode);

		while (queue.Count > 0)
		{
			AStarNode node = queue.Dequeue();
			foreach(AStarNode n in node.neighbours)
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
}
