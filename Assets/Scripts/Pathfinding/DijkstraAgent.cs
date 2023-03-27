using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraAgent : PathfinderAgent
{
	protected override PathfinderNode[,] NewMap(int width, int height)
	{
		return new DijkstraNode[width, height];
	}

	protected override PathfinderNode NewNode(int x, int y, bool walkable, Vector3 position, int id)
	{
		return new DijkstraNode(x, y, walkable, position, id);
	}

	public override List<Vector3> FindPathToLocation(Vector3 destination, bool simplify)
	{
		DijkstraNode from = (DijkstraNode)NodeFromWorldPos(transform.position);
		DijkstraNode to = (DijkstraNode)NodeFromWorldPos(destination);
		return DijkstraPathfinder.FindPath(from, to, TotalNodes, simplify);
	}

	public override List<Vector3> FindPath(Vector3 start, Vector3 destination, bool simplify)
	{
		DijkstraNode from = (DijkstraNode)NodeFromWorldPos(start);
		DijkstraNode to = (DijkstraNode)NodeFromWorldPos(destination);
		return DijkstraPathfinder.FindPath(from, to, TotalNodes, simplify);
	}
}
