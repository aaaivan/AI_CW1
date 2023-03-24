using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
	[SerializeField] GameObject[] enemyPrefabs;
	List<Transform> enemies = new List<Transform>();

	static EnemiesManager instance;
	public static EnemiesManager Instance { get { return instance; } }

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	public void SpawnEnemies()
	{
		// TODO implement
	}
}
