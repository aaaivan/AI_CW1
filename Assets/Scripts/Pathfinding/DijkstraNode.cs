using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraNode : PathfinderNode, IHeapItem<DijkstraNode>
{
	public int distance;
	int heapIndex;

	public DijkstraNode(int x, int y, bool walkable, Vector3 position, int id) :
		base(x, y, walkable, position, id)
	{ }

	public int HeapIndex
	{
		get { return heapIndex; }
		set { heapIndex = value; }
	}

	public int CompareTo(DijkstraNode other)
	{
		int compare = distance.CompareTo(other.distance);
		return -compare;
	}
}
