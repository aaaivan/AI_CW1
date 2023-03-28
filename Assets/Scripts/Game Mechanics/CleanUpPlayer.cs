using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUpPlayer : MonoBehaviour, IObjectCleanUp
{
	public void CleanUpObject()
	{
		gameObject.SetActive(false);
		GameManager.Instance.gameOver.gameObject.SetActive(true);
		GameManager.Instance.gameOver.transform.Find("Lost").gameObject.SetActive(true);
	}
}
