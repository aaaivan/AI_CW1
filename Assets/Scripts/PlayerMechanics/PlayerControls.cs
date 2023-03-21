using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
	CharacterMovement movement;
	Shooting shooting;
	bool canShoot = true;

	private void Awake()
	{
		movement = GetComponent<CharacterMovement>();
		shooting = GetComponent<Shooting>();
	}

	void Update()
    {
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		Vector3 targetDirection = new Vector3(horizontal, 0, vertical);
		if (targetDirection.magnitude > 0)
		{
			Quaternion cameraRot = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
			targetDirection = (cameraRot * targetDirection).normalized;
			movement.Move(targetDirection);
		}

		float shoot = Input.GetAxis("Fire1");
		if (canShoot && shoot == 1)
		{
			canShoot = false;
			shooting.Shoot(transform.forward);
		}
		else if(shoot == 0)
		{
			canShoot = true;
		}
	}
}
