using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public CharacterController characterController;
	public float movementSpeed = 6.0f;
	public float rotationSpeed = 10.0f;
	[HideInInspector] public Transform camera;


	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		characterController.slopeLimit = Mathf.Atan(GetComponent<AStarAgent>().pathfinderData.maxWalkableSlope) * Mathf.Rad2Deg;
	}

    void Update()
    {
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		Vector3 targetDirection = new Vector3(horizontal, 0, vertical);
		if(targetDirection.magnitude > 0)
		{
			Quaternion cameraRot = Quaternion.Euler(0, camera.eulerAngles.y, 0);
			targetDirection = (cameraRot * targetDirection).normalized;
			Quaternion targetRot = Quaternion.LookRotation(targetDirection);
			Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
			transform.rotation = rot;
			characterController.Move(targetDirection * movementSpeed * Time.deltaTime);
		}
	}
}
