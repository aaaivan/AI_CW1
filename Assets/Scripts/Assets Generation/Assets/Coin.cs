using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectableItem
{
	public string itemName;
	public string Name { get { return itemName; } }
	public Vector3 Position { get { return transform.position; } }

	public void OnObjectCollected()
	{
		Debug.Log(Name);
	}
}