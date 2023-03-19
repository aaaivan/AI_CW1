using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsGeneratorManager : MonoBehaviour
{
	public List<GameObject> assetFactoriyGameObjects;
	List<IAssetFactory> assetFactories = new List<IAssetFactory>();

	static AssetsGeneratorManager instance;
	public static AssetsGeneratorManager Instance { get { return instance; } }

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

	public void GenerateAssets(AStarAgent playerAgent, MapGenerator terrain)
	{
		List<Vector2> points = PoissonDiscSampling.GenerateDistribution(terrain.MapRect.size, 20, 50);

		foreach(var point in points)
		{
			Vector3? pos = terrain.GetPointAtCoordinates(point + terrain.MapRect.position);
			if(pos != null)
			{
				assetFactories[0].SpawnAtLocation(pos.Value, playerAgent, terrain);
			}
		}
	}

}
