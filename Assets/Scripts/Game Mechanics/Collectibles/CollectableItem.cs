using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
	[HideInInspector] public string itemName;
	[HideInInspector] public CollectableItemType itemType;
	[HideInInspector] public bool spin;
	float spinVelocity = 25f;
	PathfinderAgent playerAgent;

	private void Awake()
	{
		playerAgent = GameManager.Instance.Player.GetComponent<PathfinderAgent>();
	}

	public void Init(CollectableItemType itemType, bool spins)
	{
		this.itemType = itemType;
		this.spin = spins;
		AssetsManager.Instance.AddItem(this);
	}

	private void Update()
	{
		if (spin)
		{
			transform.Rotate(Vector3.up * spinVelocity * Time.deltaTime, Space.World);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == GameManager.Instance.Player)
		{
			OnObjectCollected(other.transform);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if(playerAgent != null)
		{
			List<Vector3> path = playerAgent.FindPathToLocation(transform.position, false);
			Gizmos.color = Color.blue;
			foreach (var n in path)
			{
				Gizmos.DrawSphere(n, MapGenerator.Instance.terrainData.uniformScale / 2);
			}
		}
	}

	protected virtual void OnObjectCollected(Transform collctedBy)
	{
		Destroy(gameObject);
	}
}
