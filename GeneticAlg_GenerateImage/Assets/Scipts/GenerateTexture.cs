using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerateTexture
{
    public static Texture2D GenerateTextureFrom2DColorArray(Color[,] colorArray2D)
    {
        int width = colorArray2D.GetLength(0);
        int height = colorArray2D.GetLength(1);
        Texture2D texture = new Texture2D(width, height);

        Color[] colorArray1D = new Color[width * height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                colorArray1D[y * width + x] = colorArray2D[x,y];
            }
        }

        texture.SetPixels(colorArray1D);
        texture.filterMode= FilterMode.Point;
        texture.wrapMode= TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
}
