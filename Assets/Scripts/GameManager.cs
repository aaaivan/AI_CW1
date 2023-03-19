using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject player;

    void Start()
    {
		// Generate terrain
        MapGenerator.Instance.DrawMapAtPosition(MapGenerator.Instance.transform.position);

		// Spawn Player
		Vector3 playerPos = MapGenerator.Instance.GetCoordinateOfNode(1, 1);
		GameObject go = Instantiate(player, playerPos, Quaternion.identity, MapGenerator.Instance.transform);

		// Spawn Items
		AssetsGeneratorManager.Instance.GenerateAssets(go.GetComponent<AStarAgent>(), MapGenerator.Instance);
	}
}
