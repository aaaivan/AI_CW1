using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : PathfinderAgent
{
	protected override PathfinderNode[,] NewMap(int width, int height)
	{
		return new AStarNode[width, height];
	}

	protected override PathfinderNode NewNode(int x, int y, bool walkable, Vector3 position, int id, int movementPenalty)
	{
		return new AStarNode(x, y, walkable, position, id, movementPenalty);
	}

	public override List<Vector3> FindPathToLocation(Vector3 destination, bool simplify)
	{
		AStarNode from = (AStarNode)NodeFromWorldPos(transform.position);
		AStarNode to = (AStarNode)NodeFromWorldPos(destination);
		return AStarPathfinder.FindPath(from, to, TotalNodes, simplify);
	}

	public override List<Vector3> FindPath(Vector3 start, Vector3 destination, bool simplify)
	{
		AStarNode from = (AStarNode)NodeFromWorldPos(start);
		AStarNode to = (AStarNode)NodeFromWorldPos(destination);
		return AStarPathfinder.FindPath(from, to, TotalNodes, simplify);
	}
}
