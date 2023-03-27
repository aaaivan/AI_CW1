using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUpEnemy : MonoBehaviour, IObjectCleanUp
{
	public void CleanUpObject()
	{
		string enemyType = GetComponent<FiniteStateMachine>().CharacterType;
		EnemiesManager.Instance.RemoveEnemy(enemyType, transform);
		Destroy(gameObject);
	}
}
