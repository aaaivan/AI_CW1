using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : IHeapItem<AStarNode>
{
	public bool walkable;
	public int x, y;
	public List<AStarNode> neighbours;

	public Vector3 position;

	public AStarNode parent = null;
	public int hCost = 0;
	public int gCost = 0;
	int heapIndex;

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

	public AStarNode(int x, int y, bool walkable, Vector3 position)
	{
		this.x = x;
		this.y = y;
		this.walkable = walkable;
		this.position = position;
	}

	public int fCost
	{
		get { return hCost + gCost; }
	}

}
