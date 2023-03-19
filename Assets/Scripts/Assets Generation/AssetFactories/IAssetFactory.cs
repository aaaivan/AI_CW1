using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAssetFactory
{
	public int ID { get; set; }
	public void SpawnAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain);
	public float GetProbabilityAtLocation(Vector3 pos, AStarAgent playerAgent, MapGenerator terrain);
}
