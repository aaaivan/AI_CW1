using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
	public static Texture2D TextureFromColourMap(Color[] colorMap, int width, int height)
	{
		Texture2D texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colorMap);
		texture.Apply();
		return texture;
	}

	public static Texture2D TextureFromHeightMap(float[,] heighMap)
	{
		int width = heighMap.GetLength(0);
		int height = heighMap.GetLength(1);

		Color[] colorMap = new Color[width * height];
		for (int i = 0; i < width * height; ++i)
		{
			int x = i % width;
			int y = i / width;
			colorMap[i] = Color.Lerp(Color.black, Color.white, heighMap[x, y]);
		}

		return TextureFromColourMap(colorMap, width, height);
	}
}
