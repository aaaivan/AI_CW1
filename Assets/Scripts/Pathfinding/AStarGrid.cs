using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
	public MapGenerator mapGenerator;
	public PathfinderData pathfinderData;
	public GameObject agent;

	AStarNode[,] nodes;
	int width;
	int height;
	float nodeDist;

	public int TotalNodes { get { return nodes.Length; } }

	private void Start()
	{
		agent = gameObject;

		nodeDist = mapGenerator.terrainData.uniformScale;
		width = mapGenerator.heightMap.GetLength(0);
		height = mapGenerator.heightMap.GetLength(1);
		CreateNodes();
	}

	void CreateNodes()
	{
		nodes = new AStarNode[width, height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				Vector3 pos = mapGenerator.GetCoordinateOfNode(x, y);
				bool walkable = !(Physics.CheckSphere(pos, nodeDist/2, pathfinderData.unwalkableLayers));
				float unscaledHeight = (pos.y - mapGenerator.transform.position.y) / nodeDist;
				walkable &= unscaledHeight <= pathfinderData.maxWalkableHeight;
				if(x == 0 || y == 0 || x == width -1  || y == height -1)
				{
					walkable = false;
				}
				nodes[x, y] = new AStarNode(x, y, walkable, pos);
			}
		}
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				nodes[x, y].neighbours = FindNeighbours(nodes[x, y]);
			}
		}
	}

	public List<AStarNode> FindNeighbours(AStarNode node)
	{
		List<AStarNode> neighbours = new List<AStarNode>();
		for (int y = node.y - 1; y <= node.y + 1; y++)
		{
			for(int x = node.x - 1; x <= node.x + 1; x++)
			{
				if (y < 0 || y >= height) break;
				if (x < 0 || x >= width) continue;
				if (x == node.x && y == node.y) continue;

				neighbours.Add(nodes[x, y]);
			}
		}
		return neighbours;
	}

	public AStarNode NodeFromWorldPos(Vector3 pos)
	{
		Vector3 lowerLeftCorner = nodes[0, 0].position;

		int x = Mathf.RoundToInt((pos.x - lowerLeftCorner.x) / nodeDist);
		x = Math.Clamp(x, 0, width - 1);
		int y = Mathf.RoundToInt((pos.z - lowerLeftCorner.z) / nodeDist);
		y = Math.Clamp(y, 0, height - 1);

		return nodes[x, y];
	}

	public List<AStarNode> path = new List<AStarNode>();
	private void OnDrawGizmos()
	{
		if (nodes != null)
		{
			AStarNode playerNode = NodeFromWorldPos(agent.transform.position);
			foreach (var n in nodes)
			{
				Gizmos.color = n.walkable ? Color.white : Color.red;
				if(path.Contains(n))
				{
					Gizmos.color = Color.blue;
				}
				Gizmos.DrawSphere(n.position, nodeDist / 2);
			}
		}
	}
}
