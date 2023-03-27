using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
	public static List<Vector3> FindPath(AStarNode startNode, AStarNode targetNode, int totalNodes, bool simplifyPath = false)
	{
		Heap<AStarNode> openList = new Heap<AStarNode>(totalNodes);
		HashSet<AStarNode> closedList = new HashSet<AStarNode>();

		startNode.gCost = 0;
		openList.Add(startNode);
		bool success = false;

		if(startNode.accessible &&  targetNode.accessible)
		{
			while (openList.Count > 0)
			{
				// find node with lowest fcost
				AStarNode currentNode = openList.Pop();
				// move node to closed list
				closedList.Add(currentNode);

				if (currentNode == targetNode)
				{
					success = true;
					break;
				}
				else
				{
					// Update the hCost and gCost of the neighbours
					foreach (AStarNode node in currentNode.neighbours)
					{
						if (!node.accessible || closedList.Contains(node)) continue;

						int newCost = currentNode.gCost + PathfinderUtils.GetDistance(currentNode, node) + node.movementPenalty;
						if (newCost < node.gCost || !openList.Contains(node))
						{
							node.gCost = newCost;
							node.hCost = PathfinderUtils.GetDistance(node, targetNode);
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
