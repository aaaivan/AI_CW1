using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DijkstraPathfinder
{
	public static List<PathfinderNode> FindPath(DijkstraNode startNode, DijkstraNode targetNode, int totalNodes)
	{
		Heap<DijkstraNode> openList = new Heap<DijkstraNode>(totalNodes);
		HashSet<DijkstraNode> closedList = new HashSet<DijkstraNode>();

		startNode.distance = 0;
		openList.Add(startNode);

		while (openList.Count > 0)
		{
			// find node with lowest distance
			DijkstraNode currentNode = openList.Pop();
			// move node to closed list
			closedList.Add(currentNode);

			if (currentNode == targetNode)
			{
				return RetracePath(startNode, targetNode);
			}
			else
			{
				// Update the distances of the neighbours
				foreach (DijkstraNode node in currentNode.neighbours)
				{
					if (!node.walkable || closedList.Contains(node)) continue;

					int newCost = currentNode.distance + GetDistance(currentNode, node);
					if (newCost < node.distance || !openList.Contains(node))
					{
						node.distance = newCost;
						node.parent = currentNode;
						if (!openList.Contains(node))
						{
							openList.Add(node);
						}
						else
						{
							openList.UpdateItem(node);
						}
					}
				}
			}
		}

		return new List<PathfinderNode>();
	}

	static List<PathfinderNode> RetracePath(DijkstraNode startNode, DijkstraNode targetNode)
	{
		List<PathfinderNode> path = new List<PathfinderNode>();
		PathfinderNode node = targetNode;
		while (node != startNode && node != null)
		{
			path.Add(node);
			node = node.parent;
		}
		path.Reverse();
		return path;
	}

	static int GetDistance(DijkstraNode fromNode, DijkstraNode toNode)
	{
		int dx = Mathf.Abs(fromNode.x - toNode.x);
		int dy = Mathf.Abs(fromNode.y - toNode.y);
		int min = Mathf.Min(dx, dy);
		int max = Mathf.Max(dx, dy);

		int dist = 14 * min + 10 * (max - min);
		return dist;
	}
}
