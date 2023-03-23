using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
	[HideInInspector] public string itemName;
	[HideInInspector] public CollectableItemType itemType;
	[HideInInspector] public bool spin;
	float spinVelocity = 25f;
	AStarAgent playerAgent;

	private void Awake()
	{
		playerAgent = GameManager.Instance.Player.GetComponent<AStarAgent>();
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
			OnObjectCollected();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if(playerAgent != null)
		{
			List<PathfinderNode> path = playerAgent.FindPathToLocation(transform.position);
			Gizmos.color = Color.blue;
			foreach (var n in path)
			{
				Gizmos.DrawSphere(n.position, MapGenerator.Instance.terrainData.uniformScale / 2);
			}
		}
	}

	protected virtual void OnObjectCollected()
	{
		Destroy(gameObject);
	}
}
