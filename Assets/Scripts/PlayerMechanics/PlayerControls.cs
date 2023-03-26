using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
	[SerializeField] float mouseSensitivityX = 5;
	[SerializeField] float mouseSensitivityY = 5;
	[SerializeField] float cameraClampAngleBtm = 30;
	[SerializeField] float cameraClampAngleTop = 70;
	[SerializeField] LayerMask aimLayerMask;
	float cameraPitch = 0;

	Transform cameraLocation;
	Transform projectileSpawnLocation;
	CharacterMovement movement;
	Shooting shooting;
	bool canShoot = true;

	private void Awake()
	{
		cameraLocation = transform.Find("CameraFollow");
		projectileSpawnLocation = transform.Find("BulletSpawnPos");
		movement = GetComponent<CharacterMovement>();
		shooting = GetComponent<Shooting>();

		// set the slow limit so that it matches the one specified in the pathfinder
		CharacterController characterController = GetComponent<CharacterController>();
		characterController.slopeLimit = Mathf.Atan(GetComponent<PathfinderAgent>().pathfinderData.maxWalkableSlope) * Mathf.Rad2Deg;
	}

	void Update()
    {
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		float rotation = Input.GetAxisRaw("Mouse X");
		float lookUpDown = Input.GetAxisRaw("Mouse Y");
		
		Vector3 targetDirection = new Vector3(horizontal, 0, vertical);
		if (targetDirection.magnitude > 0)
		{
			Quaternion cameraRot = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
			targetDirection = (cameraRot * targetDirection).normalized;
			movement.Move(targetDirection);
		}
		// rotate player left/right
		movement.Rotate(mouseSensitivityX * rotation);

		// rotate camera up/down
		cameraPitch -= lookUpDown * mouseSensitivityY;
		cameraPitch = Mathf.Clamp(cameraPitch, -cameraClampAngleTop, cameraClampAngleBtm);
		cameraLocation.transform.localRotation = Quaternion.Euler(cameraPitch, 0.0f, 0.0f);

		float shoot = Input.GetAxis("Fire1");
		if (canShoot && shoot == 1)
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2f, Screen.height/2f));
			if(Physics.Raycast(ray, out RaycastHit hit, 999f, aimLayerMask, QueryTriggerInteraction.Ignore))
			{
				shooting.Shoot(hit.point - projectileSpawnLocation.position);
			}
			else
			{
				shooting.Shoot(ray.direction);
			}
			canShoot = false;
		}
		else if(shoot == 0)
		{
			canShoot = true;
		}
	}
}
