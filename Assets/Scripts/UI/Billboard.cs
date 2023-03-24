using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	Camera cam;

	private void Awake()
	{
		cam = Camera.main;
	}

	private void LateUpdate()
	{
		transform.LookAt(transform.position + cam.transform.forward);
	}
}
