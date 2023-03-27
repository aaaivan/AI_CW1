using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : PathfinderNode, IHeapItem<AStarNode>
{
	public int hCost = 0;
	public int gCost = 0;
	int heapIndex;

	public AStarNode(int x, int y, bool walkable, Vector3 position, int id, int movementPenalty) :
		base(x, y, walkable, position, id, movementPenalty)
	{ }

	public int HeapIndex
	{
		get { return heapIndex; }
		set { heapIndex = value; }
	}

	public int CompareTo(AStarNode other)
	{
		int compare = fCost.CompareTo(other.fCost);
		if(compare == 0)
		{
			compare = hCost.CompareTo(other.hCost);
		}
		return -compare;
	}

	int fCost
	{
		get { return hCost + gCost; }
	}
}
