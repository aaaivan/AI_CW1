using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderNode
{
	public bool walkable;
	public bool accessible;
	public int movementPenalty;
	public int x, y;
	public int id;
	public List<PathfinderNode> neighbours;
	public PathfinderNode parent = null;
	public Vector3 position;
	public Vector2 gradient;

	public PathfinderNode(int x, int y, bool walkable, Vector3 position, int id, int movementPenalty)
	{
		this.x = x;
		this.y = y;
		this.walkable = walkable;
		this.accessible = true;
		this.position = position;
		this.id = id;
		this.movementPenalty = movementPenalty;
	}
}
