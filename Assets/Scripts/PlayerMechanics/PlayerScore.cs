using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
	int currentScore = 0;
	public int Score { get { return currentScore; } }
	
	public void AddScore(int points)
	{
		currentScore += points;
	}
}
