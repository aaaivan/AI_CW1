using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject playerPrefab;
	public CinemachineVirtualCamera cinemachine;
	public Camera mainCamera;

	GameObject player;
	public GameObject Player { get { return player; } }

	static GameManager instance;
	public static GameManager Instance { get { return instance; } }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	void Start()
    {
		// Generate terrain
        MapGenerator.Instance.DrawMapAtPosition(MapGenerator.Instance.transform.position);

		// Spawn Player
		Vector3 playerPos = MapGenerator.Instance.GetCoordinateOfNode(1, 1);
		player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
		cinemachine.Follow = player.transform.Find("CameraFollow");

		// Spawn Items
		AssetsGeneratorManager.Instance.GenerateAssets(player.GetComponent<PathfinderAgent>());

		// Spawn Enemies
		EnemiesManager.Instance.SpawnEnemies();
	}
}
