using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ICollectableItem;

public class Tree : MonoBehaviour, ICollectableItem
{
	public ItemType type;
	public ItemType Type { get { return type; } }
	public Vector3 Position { get { return transform.position; } }

	public void OnObjectCollected()
	{
		Debug.Log(AssetsManager.Instance.ItemNameFromEnum(type));
	}
}
