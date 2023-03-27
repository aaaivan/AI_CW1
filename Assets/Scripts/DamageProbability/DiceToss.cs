using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiceToss
{
	public static int TossDice(int numberOfTosses, int maxDieValue)
	{
		int total = 0;
		for(int i = 0; i < numberOfTosses; i++)
		{
			total += Random.Range(0, maxDieValue) + 1;
		}

		return total;
	}

}
