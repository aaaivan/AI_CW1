using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
	[HideInInspector] public string itemName;
	[HideInInspector] public CollectableItemType itemType;
	[HideInInspector] public bool spin;
	float spinVelocity = 25f;

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

	protected virtual void OnObjectCollected()
	{
		Destroy(gameObject);
	}
}
