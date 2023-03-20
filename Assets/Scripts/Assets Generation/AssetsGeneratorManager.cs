using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class AssetsGeneratorManager : MonoBehaviour
{
	public List<GameObject> assetFactoriyGameObjects;
	public float minDistanceBetweenItems = 10;
	public int iterationsBeforeRejection = 100;

	List<IAssetFactory> assetFactories = new List<IAssetFactory>();

	static AStarAgent lastAgentUsed = null;
	static AssetsGeneratorManager instance;
	public static AssetsGeneratorManager Instance { get { return instance; }}

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			foreach(var go in assetFactoriyGameObjects)
			{
				IAssetFactory f = go.GetComponent<IAssetFactory>();
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
		GenerateAssets(lastAgentUsed);
	}

	public void GenerateAssets(AStarAgent agent)
	{
		if (agent == null)
			return;

		lastAgentUsed = agent;
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
