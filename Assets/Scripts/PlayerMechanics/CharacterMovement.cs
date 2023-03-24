using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	public float movementSpeed = 6.0f;
	public float maxRotationSpeed = 10.0f;
	public float gravity = -9.81f;
	public float groundCheckRadius = 0.4f;
	public LayerMask groundMask;

	CharacterController characterController;

	float verticalVelocity = 0;
	bool grounded = false;


	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		characterController.slopeLimit = Mathf.Atan(GetComponent<PathfinderAgent>().pathfinderData.maxWalkableSlope) * Mathf.Rad2Deg;
	}

    void Update()
    {
		verticalVelocity += gravity * Time.deltaTime;
		characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
		GroundedCheck();
	}

	public void Move(Vector3 deltaPosition)
	{
		if (deltaPosition.magnitude > 0)
		{
			characterController.Move(deltaPosition * movementSpeed * Time.deltaTime);
		}
	}

	public void Rotate(float deltaRot)
	{
		if (deltaRot != 0)
		{
			deltaRot = Mathf.Clamp(deltaRot, -maxRotationSpeed * Time.deltaTime, maxRotationSpeed * Time.deltaTime);
			transform.Rotate(Vector3.up, deltaRot);
		}
	}

	private void GroundedCheck()
	{
		grounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
		// stop our velocity dropping infinitely when grounded
		if (grounded && verticalVelocity < 0.0f)
		{
			verticalVelocity = -2f;
		}
	}
}
