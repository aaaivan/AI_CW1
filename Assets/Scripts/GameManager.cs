using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject playerPrefab;
	public CinemachineVirtualCamera cinemachine;
	public Camera mainCamera;
	public DisplayPlayerHP healthUI;
	public DisplayPlayerScore playerScoreUI;
	public RectTransform gameOver;
	public DisplayPlayerScore endgamePlayerScoreUI;

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
		Rect terrain = MapGenerator.Instance.MapInnerRect;
		Vector3 playerPos = MapGenerator.Instance.GetPointAtCoordinates(new Vector2(
			terrain.x + terrain.width/2,
			terrain.y)).Value;
		player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
		cinemachine.Follow = player.transform.Find("CameraFollow");

		// Pass player reference to scoreboard
		healthUI.SetPlayer(player.GetComponent<DamageableObject>());
		playerScoreUI.SetPlayerScore(player.GetComponent<PlayerScore>());
		endgamePlayerScoreUI.SetPlayerScore(player.GetComponent<PlayerScore>());

		// Spawn Items
		AssetsGeneratorManager.Instance.GenerateAssets(player.GetComponent<PathfinderAgent>());

		// Spawn Enemies
		EnemiesManager.Instance.SpawnEnemies();
	}
}
