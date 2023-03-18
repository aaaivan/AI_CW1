using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
	public static List<AStarNode> FindPath(AStarNode startNode, AStarNode targetNode, int totalNodes)
	{
		Heap<AStarNode> openList = new Heap<AStarNode>(totalNodes);
		HashSet<AStarNode> closedList = new HashSet<AStarNode>();

		startNode.gCost = 0;
		openList.Add(startNode);

		while(openList.Count > 0)
		{
			// find node with lowest fcost
			AStarNode currentNode = openList.Pop();
			// move node to closed list
			closedList.Add(currentNode);

			if (currentNode == targetNode)
			{
				return RetracePath(startNode, targetNode);
			}
			else
			{
				// Update the hCost and gCost of the neighbours
				foreach (AStarNode node in currentNode.neighbours)
				{
					if (!node.walkable || closedList.Contains(node)) continue;
					
					int newCost = currentNode.gCost + GetDistance(currentNode, node);
					if(newCost < node.gCost || !openList.Contains(node))
					{
						node.gCost = newCost;
						node.hCost = GetDistance(node, targetNode);
						node.parent = currentNode;
						if(!openList.Contains(node))
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

		return new List<AStarNode>();
	}

	static List<AStarNode> RetracePath(AStarNode startNode, AStarNode targetNode)
	{
		List <AStarNode> path =  new List<AStarNode>();
		AStarNode node = targetNode;
		while(node != startNode && node != null)
		{
			path.Add(node);
			node = node.parent;
		}
		path.Reverse();
		return path;
	}

	static int GetDistance(AStarNode fromNode, AStarNode toNode)
	{
		int dx = Mathf.Abs(fromNode.x - toNode.x);
		int dy = Mathf.Abs(fromNode.y - toNode.y);
		int min = Mathf.Min(dx, dy);
		int max = Mathf.Max(dx, dy);

		int dist = 14 * min + 10 * (max - min);
		return dist;
	}
}
