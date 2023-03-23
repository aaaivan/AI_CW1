using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : PathfinderAgent
{
	protected override PathfinderNode[,] NewMap(int width, int height)
	{
		return new AStarNode[width, height];
	}

	protected override PathfinderNode NewNode(int x, int y, bool walkable, Vector3 position, int id)
	{
		return new AStarNode(x, y, walkable, position, id);
	}

	public override List<PathfinderNode> FindPathToLocation(Vector3 destination)
	{
		AStarNode from = (AStarNode)NodeFromWorldPos(transform.position);
		AStarNode to = (AStarNode)NodeFromWorldPos(destination);
		return AStarPathfinder.FindPath(from, to, TotalNodes);
	}
}
