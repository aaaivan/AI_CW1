using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class AssetsGeneratorManager : MonoBehaviour
{
	public List<GameObject> assetFactoriyGameObjects;
	public float minDistanceBetweenItems = 10;
	int iterationsBeforeRejection = 30;

	List<AssetFactory> assetFactories = new List<AssetFactory>();

	static AssetsGeneratorManager instance;
	public static AssetsGeneratorManager Instance { get { return instance; }}

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			foreach(var go in assetFactoriyGameObjects)
			{
				AssetFactory f = go.GetComponent<AssetFactory>();
				if (f != null)
				{
					assetFactories.Add(f);
				}
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		if(instance == this)
		{
			instance = null;
		}
	}

	public void GenerateAssets()
	{
		GenerateAssets(GameManager.Instance.Player.GetComponent<PathfinderAgent>());
	}

	public void GenerateAssets(PathfinderAgent agent)
	{
		if (agent == null)
			return;

		MapGenerator terrain = MapGenerator.Instance;

		// clear existing items
		AssetsManager.Instance.ClearItems();

		// generate items
		List<Vector2> points = PoissonDiscSampling.GenerateDistribution(terrain.MapInnerRect.size, minDistanceBetweenItems, iterationsBeforeRejection);
		foreach(var point in points)
		{
			Vector3? pos = terrain.GetPointAtCoordinates(point + terrain.MapInnerRect.position);
			if (pos == null) continue;

			float probabilityScaleFactor = 0;
			List<float> cumulativeProbability = new List<float>();
			foreach (var f in assetFactories)
			{
				float p = f.GetProbabilityAtLocation(pos.Value, agent, terrain);
				probabilityScaleFactor += p;
				cumulativeProbability.Add(probabilityScaleFactor);
			}
			if (probabilityScaleFactor <= 0) continue;

			float r = Random.value * probabilityScaleFactor;
			for (int i = 0; i < assetFactories.Count; i++)
			{
				if (cumulativeProbability[i] > r)
				{
					assetFactories[i].SpawnAtLocation(pos.Value, agent, terrain);
					break;
				}
			}
		}

		// Calculate clusters
		AssetsManager.Instance.CalculateClusters();
	}

	private void OnValidate()
	{
		if(minDistanceBetweenItems < 1)
		{
			minDistanceBetweenItems = 1;
		}
	}

}
