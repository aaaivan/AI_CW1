using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPlayerScore : MonoBehaviour
{
	TMP_Text text;
	string textFormat = "Score: {0}";
	PlayerScore playerScore;

	void Awake()
	{
		text = GetComponent<TMP_Text>();
	}

	void Update()
	{
		text.text = string.Format(textFormat, playerScore.Score);
	}

	public void SetPlayerScore(PlayerScore score)
	{
		playerScore = score;
	}
}
