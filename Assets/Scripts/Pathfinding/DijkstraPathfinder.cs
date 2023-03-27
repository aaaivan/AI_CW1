using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DijkstraPathfinder
{
	public static List<Vector3> FindPath(DijkstraNode startNode, DijkstraNode targetNode, int totalNodes, bool simplifyPath = false)
	{
		Heap<DijkstraNode> openList = new Heap<DijkstraNode>(totalNodes);
		HashSet<DijkstraNode> closedList = new HashSet<DijkstraNode>();

		startNode.distance = 0;
		openList.Add(startNode);

		bool success = false;
		if(startNode.accessible && targetNode.accessible)
		{
			while (openList.Count > 0)
			{
				// find node with lowest distance
				DijkstraNode currentNode = openList.Pop();
				// move node to closed list
				closedList.Add(currentNode);

				if (currentNode == targetNode)
				{
					success = true;
					break;
				}
				else
				{
					// Update the distances of the neighbours
					foreach (DijkstraNode node in currentNode.neighbours)
					{
						if (!node.accessible || closedList.Contains(node)) continue;

						int newCost = currentNode.distance + PathfinderUtils.GetDistance(currentNode, node) + node.movementPenalty;
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
		}

		if(success)
		{
			return PathfinderUtils.RetracePath(startNode, targetNode, simplifyPath);
		}
		else
		{
			return new List<Vector3>();
		}
	}
}
