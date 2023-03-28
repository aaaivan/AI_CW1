using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
	}

    void Update()
    {
		verticalVelocity += gravity * Time.deltaTime;
		characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
		GroundedCheck();
	}

	public void Move(Vector3 moveDirection)
	{
		if (moveDirection.magnitude > 0)
		{
			characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
		}
	}

	public void MoveTowards(Vector3 targetPoint, float speedMultiplier = 1.0f)
	{
		Vector3 pos = transform.position;
		MoveAndRotate((targetPoint - transform.position).normalized * speedMultiplier);
		if(Mathf.Sign(transform.position.x - targetPoint.x) != Mathf.Sign(pos.x - targetPoint.x))
		{
			transform.position = new Vector3(targetPoint.x, transform.position.y, transform.position.z);
		}
		if (Mathf.Sign(transform.position.z - targetPoint.z) != Mathf.Sign(pos.z - targetPoint.z))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, targetPoint.z);
		}
	}

	public void MoveAndRotate(Vector3 moveDirection)
	{
		if (moveDirection.magnitude > 0)
		{
			characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
			SetLookDirection(moveDirection);
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

	public void RotateTowards(Vector3 point)
	{
		Vector3 direction = point - transform.position;
		SetLookDirection(direction);
	}

	public void RotateTowardsUnclamped(Vector3 point, float angularVelocity)
	{
		Vector3 direction = point - transform.position;
		Vector3 fwd = new Vector3(transform.forward.x, 0, transform.forward.z);
		Vector3 targetLookDir = new Vector3(direction.x, 0, direction.z);
		float angle = Vector3.SignedAngle(fwd, targetLookDir, Vector3.up);
		angle = Mathf.Clamp(angle, -angularVelocity * Time.deltaTime, angularVelocity * Time.deltaTime);
		transform.Rotate(Vector3.up, angle);
	}

	public void SetLookDirection(Vector3 direction)
	{
		Vector3 fwd = new Vector3(transform.forward.x, 0, transform.forward.z);
		Vector3 targetLookDir = new Vector3(direction.x, 0, direction.z);
		float angle = Vector3.SignedAngle(fwd, targetLookDir, Vector3.up);
		Rotate(angle);
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
