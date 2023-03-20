using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float movementSpeed = 6.0f;
	public float rotationSpeed = 10.0f;
	public float gravity = -9.81f;
	public float groundCheckRadius = 0.4f;
	public LayerMask groundMask;


	CharacterController characterController;
	[HideInInspector] public Transform thirdPersonCamera;

	float verticalVelocity = 0;
	bool grounded = false;


	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		characterController.slopeLimit = Mathf.Atan(GetComponent<AStarAgent>().pathfinderData.maxWalkableSlope) * Mathf.Rad2Deg;
	}

    void Update()
    {
		verticalVelocity += gravity * Time.deltaTime;
		characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
		GroundedCheck();

		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		Vector3 targetDirection = new Vector3(horizontal, 0, vertical);
		if(targetDirection.magnitude > 0)
		{
			Quaternion cameraRot = Quaternion.Euler(0, thirdPersonCamera.eulerAngles.y, 0);
			targetDirection = (cameraRot * targetDirection).normalized;
			Quaternion targetRot = Quaternion.LookRotation(targetDirection);
			Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
			transform.rotation = rot;
			characterController.Move(targetDirection * movementSpeed * Time.deltaTime);
		}

	}

	private void GroundedCheck()
	{
		grounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
		// stop our velocity dropping infinitely when grounded
		if (verticalVelocity < 0.0f)
		{
			verticalVelocity = -2f;
		}
	}
}
