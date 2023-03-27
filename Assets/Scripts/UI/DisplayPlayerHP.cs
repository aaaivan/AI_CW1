using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPlayerHP : MonoBehaviour
{
	TMP_Text text;
	string textFormat = "HP: {0}";
	DamageableObject health;

    void Awake()
    {
		text = GetComponent<TMP_Text>();
    }

    void Update()
    {
		text.text = string.Format(textFormat, health.CurrentHealth);
    }

	public void SetPlayer(DamageableObject player)
	{
		health = player;
	}
}
