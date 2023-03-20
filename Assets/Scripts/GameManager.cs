using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject player;
	public CinemachineFreeLook cinemachine;
	public Camera mainCamera;

    void Start()
    {
		// Generate terrain
        MapGenerator.Instance.DrawMapAtPosition(MapGenerator.Instance.transform.position);

		// Spawn Player
		Vector3 playerPos = MapGenerator.Instance.GetCoordinateOfNode(1, 1);
		GameObject go = Instantiate(player, playerPos, Quaternion.identity, MapGenerator.Instance.transform);
		cinemachine.Follow = go.transform.Find("CameraLookAt");
		cinemachine.LookAt = go.transform.Find("CameraLookAt");
		go.GetComponent<PlayerMovement>().camera = mainCamera.transform;

		// Spawn Items
		AssetsGeneratorManager.Instance.GenerateAssets(go.GetComponent<AStarAgent>());
	}
}
