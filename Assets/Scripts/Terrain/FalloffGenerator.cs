using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
	public static float[,] GenerateFalloutMap(int size, float falloffSlope, float falloffDistance)
	{
		float[,] map = new float[size, size];
		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				float x = i / (float)size * 2 - 1;
				float y = j / (float)size * 2 - 1;
				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				map[i, j] = Mathf.Clamp01(1 - Evaluate(value, falloffSlope, falloffDistance));
			}
		}
		return map;
	}

	static float Evaluate(float value, float a, float b)
	{
		if (b == 0) return 1;

		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
	}
}
