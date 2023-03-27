using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUpPlayer : MonoBehaviour, IObjectCleanUp
{
	public void CleanUpObject()
	{
		gameObject.SetActive(false);
	}
}
