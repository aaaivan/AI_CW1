using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
	public MapGenerator mapGenerator;
	public PathfinderData pathfinderData;
	public GameObject agent;

	AStarNode[,] gridNodes;
	int width;
	int height;
	float nodeDist;

	public int TotalNodes { get { return gridNodes.Length; } }

	private void Start()
	{
		agent = gameObject;
		CreateNodes();
	}

	void CreateNodes()
	{
		nodeDist = mapGenerator.terrainData.uniformScale;
		width = mapGenerator.heightMap.GetLength(0);
		height = mapGenerator.heightMap.GetLength(1);

		gridNodes = new AStarNode[width, height];
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
				gridNodes[x, y] = new AStarNode(x, y, walkable, pos);
			}
		}
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				gridNodes[x, y].neighbours = FindNeighbours(gridNodes[x, y]);
			}
		}
	}

	public List<AStarNode> FindNeighbours(AStarNode node)
	{
		List<AStarNode> neighbours = new List<AStarNode>();
		float? slopeX = null;
		float? slopeY = null;
		for (int y = node.y - 1; y <= node.y + 1; y++)
		{
			for(int x = node.x - 1; x <= node.x + 1; x++)
			{
				if (y < 0 || y >= height) break;
				if (x < 0 || x >= width) continue;
				if (x == node.x && y == node.y) continue;

				AStarNode n = gridNodes[x, y];
				neighbours.Add(n);

				if (node.x == n.x)
				{
					float slope = (n.position.y - node.position.y) / (n.position.z - node.position.z);
					if (slopeX == null)
					{
						slopeX = slope;
					}
					else if (slopeX != 0 && slope != 0 &&
						Mathf.Sign((float)slopeX) != Mathf.Sign(slope))
					{
						slopeX = 0;
					}
					else
					{
						slopeX = ((float)slopeX + slope) / 2;
					}
				}
				else if (node.y == n.y)
				{
					float slope = (n.position.y - node.position.y) / (n.position.x - node.position.x);
					if (slopeY == null)
					{
						slopeY = slope;
					}
					else if (slopeY != 0 && slope != 0 &&
						Mathf.Sign((float)slopeY) != Mathf.Sign(slope))
					{
						slopeY = 0;
					}
					else
					{
						slopeY = ((float)slopeY + slope) / 2;
					}
				}
			}
		}

		Vector2 gradient = new Vector2((float)slopeX, (float)slopeY);
		if (gradient.magnitude > pathfinderData.maxWalkableSlope)
		{
			node.walkable = false;
		}

		return neighbours;
	}

	public AStarNode NodeFromWorldPos(Vector3 pos)
	{
		Vector3 lowerLeftCorner = gridNodes[0, 0].position;

		int x = Mathf.RoundToInt((pos.x - lowerLeftCorner.x) / nodeDist);
		x = Math.Clamp(x, 0, width - 1);
		int y = Mathf.RoundToInt((pos.z - lowerLeftCorner.z) / nodeDist);
		y = Math.Clamp(y, 0, height - 1);

		return gridNodes[x, y];
	}

	void OnTerrainDataUpdated()
	{
		if(mapGenerator != null)
		{
			CreateNodes();
		}
	}

	private void OnValidate()
	{
		if (pathfinderData != null)
		{
			pathfinderData.OnValuesUpdated -= OnTerrainDataUpdated;
			pathfinderData.OnValuesUpdated += OnTerrainDataUpdated;
		}
	}

	public List<AStarNode> path = new List<AStarNode>();
	private void OnDrawGizmos()
	{
		if (gridNodes != null)
		{
			AStarNode playerNode = NodeFromWorldPos(agent.transform.position);
			foreach (var n in gridNodes)
			{
				Gizmos.color = n.walkable ? Color.white : Color.red;
				if(path.Contains(n))
				{
					Gizmos.color = Color.blue;
				}
				Gizmos.DrawSphere(n.position, nodeDist / 4);
			}
		}
	}
}
