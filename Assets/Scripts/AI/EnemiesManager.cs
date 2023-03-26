using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
	[SerializeField] List<GameObject> enemyPrefabs;
	[SerializeField] List<int> numberOfEachEnemyType;
	[SerializeField] float minDstanceBwteenEnemies;
	int iterationsBeforeRejection = 30;

	Dictionary<string, List<Transform>> enemies = new Dictionary<string, List<Transform>>();

	static EnemiesManager instance;
	public static EnemiesManager Instance { get { return instance; } }

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			int typesOfEnemies = Mathf.Max(numberOfEachEnemyType.Count, enemyPrefabs.Count);
			if(enemyPrefabs.Count > typesOfEnemies)
			{
				enemyPrefabs.RemoveRange(typesOfEnemies, enemyPrefabs.Count - typesOfEnemies);
			}
			if (numberOfEachEnemyType.Count > typesOfEnemies)
			{
				numberOfEachEnemyType.RemoveRange(typesOfEnemies, numberOfEachEnemyType.Count - typesOfEnemies);
			}
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
		MapGenerator terrain = MapGenerator.Instance;
		Vector2 seed = new Vector2(terrain.MapInnerRect.width / 2, terrain.MapInnerRect.yMax);
		List<Vector2> points = PoissonDiscSampling.GenerateDistribution(terrain.MapInnerRect.size,
			minDstanceBwteenEnemies, iterationsBeforeRejection, seed);

		int totalEnemies = 0;
		List<int> enemiesCounts = new List<int>(numberOfEachEnemyType);
		foreach (int i in numberOfEachEnemyType)
		{
			totalEnemies += i;
		}

		foreach (var point in points)
		{
			if (totalEnemies == 0) break;

			Vector3? pos = terrain.GetPointAtCoordinates(point + terrain.MapInnerRect.position);
			if (pos == null) continue;

			int r = Random.Range(0, totalEnemies);
			int t = 0;
			for(int i = 0; i < enemiesCounts.Count;  i++)
			{
				t += enemiesCounts[i];
				if(t > r)
				{
					totalEnemies--;
					enemiesCounts[i]--;
					GameObject go = Instantiate(enemyPrefabs[i], new Vector3(seed.x, 3, seed.y), Quaternion.identity);
					go.transform.position = go.GetComponent<PathfinderAgent>().ClosestAccessibleLocation(pos.Value);

					string enemyType = go.GetComponent<FiniteStateMachine>().CharacterType;
					if(!enemies.ContainsKey(enemyType))
					{
						enemies[enemyType] = new List<Transform>();
					}
					enemies[enemyType].Add(go.transform);
					break;
				}
			}
		}
	}

	public List<Transform> GetEnemiesByFSM(string fsmName)
	{
		if(enemies.ContainsKey(fsmName))
		{
			return enemies[fsmName];
		}

		return new List<Transform>();
	}
}
