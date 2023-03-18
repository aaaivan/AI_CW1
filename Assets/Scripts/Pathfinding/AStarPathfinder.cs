using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
	//public GameObject seeker;
	//public GameObject target;

	AStarGrid grid;

	private void Awake()
	{
		grid = GetComponent<AStarGrid>();
	}

	private void Update()
	{
		//grid.path = FindPath(seeker.transform.position, target.transform.position);
	}

	public List<AStarNode> FindPath(Vector3 from, Vector3 to)
	{
		AStarNode startNode = grid.NodeFromWorldPos(from);
		AStarNode targetNode = grid.NodeFromWorldPos(to);

		Heap<AStarNode> openList = new Heap<AStarNode>(grid.TotalNodes);
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
					
					float newCost = currentNode.gCost + GetDistance(currentNode, node);
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

	List<AStarNode> RetracePath(AStarNode startNode, AStarNode targetNode)
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

	float GetDistance(AStarNode fromNode, AStarNode toNode)
	{
		float dx = Mathf.Abs(fromNode.x - toNode.x);
		float dy = Mathf.Abs(fromNode.y - toNode.y);
		float min = Mathf.Min(dx, dy);
		float max = Mathf.Max(dx, dy);

		float dist = 1.4f * min + 1.0f * (max - min);
		//dist += grid.pathfinderData.GetCostForSlope(Vector3.Angle(Vector3.forward, (to - from)));

		return dist;
	}
}
