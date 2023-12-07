using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DNA
{
    public float fitness;

    int width, height;
    public Color[,] pixelColors;

    int mutationIncrementRange = 5;

    public DNA(int width, int height)
    {
        this.width = width;
        this.height = height;
        pixelColors = new Color[width, height];

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Color randomColor = new Color(Random.value, Random.value, Random.value, Random.value);
                pixelColors[x, y] = randomColor;
            }
        }
    }

    public void calcFitness(Texture2D target)
    {
        // Calc fitness based on how close this element's rgba is to the target image's rgba
        float score = 0;
        for(int y = 0; y < height; y++)
        { 
            for(int x = 0; x < width; x++)
            {
                // Get 1 - (difference of rgba values) to get sort of a percentage of how close the value is to target value
                // and make higher percentage have exponentially more impact
                float rDiff = 1 - Mathf.Abs(target.GetPixel(x, y).r - pixelColors[x, y].r);
                rDiff = Mathf.Pow(rDiff, 1/10);
                float gDiff = 1 - Mathf.Abs(target.GetPixel(x, y).g - pixelColors[x, y].g);
                gDiff = Mathf.Pow(gDiff, 1/10);
                float bDiff = 1 - Mathf.Abs(target.GetPixel(x, y).b - pixelColors[x, y].b);
                bDiff = Mathf.Pow(bDiff, 1/10);
                float aDiff = 1 - Mathf.Abs(target.GetPixel(x, y).a - pixelColors[x, y].a);
                aDiff = Mathf.Pow(aDiff, 1/10);
                float avgPixelDiff = (rDiff + gDiff + bDiff + aDiff) / 4;
                score += avgPixelDiff;
            }
        }
        float avgTargetDiff = score / (width * height);
        fitness = Mathf.Pow(avgTargetDiff, 10);
    }

    public DNA crossover(DNA o, Texture2D target)
    {
        DNA child = new DNA(width, height);

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                // Get better rgba value from parents
                Color rgbaDiffParentA = getRGBADiff(this.pixelColors[x,y], target.GetPixel(x,y));
                Color rgbaDiffParentB = getRGBADiff(o.pixelColors[x,y], target.GetPixel(x, y));

                Color betterRGBA;

                if (rgbaDiffParentA.r < rgbaDiffParentB.r)
                    betterRGBA.r = pixelColors[x, y].r;
                else
                    betterRGBA.r = o.pixelColors[x, y].r;
                if (rgbaDiffParentA.g < rgbaDiffParentB.g)
                    betterRGBA.g = pixelColors[x, y].g;
                else
                    betterRGBA.g = o.pixelColors[x, y].g;
                if (rgbaDiffParentA.b < rgbaDiffParentB.b)
                    betterRGBA.b = pixelColors[x, y].b;
                else
                    betterRGBA.b = o.pixelColors[x, y].b;
                if (rgbaDiffParentA.a < rgbaDiffParentB.a)
                    betterRGBA.a = pixelColors[x, y].a;
                else
                    betterRGBA.a = o.pixelColors[x, y].a;

                child.pixelColors[x, y] = betterRGBA;
            }
        }
        return child;
    }

    public void mutate(float mutationRate)
    {
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                if(Random.value < mutationRate)
                {
                    // Chance of mutation occurs
                    for(int i = 0; i < 4; i++)
                    {
                        int mutationIncrement = Random.Range(-mutationIncrementRange, mutationIncrementRange);
                        switch(i)
                        {
                            case 0:
                                pixelColors[x, y].r += mutationIncrement;
                                break;
                            case 1:
                                pixelColors[x, y].g += mutationIncrement;
                                break;
                            case 2:
                                pixelColors[x, y].b += mutationIncrement;
                                break;
                            case 3:
                                pixelColors[x, y].a += mutationIncrement;
                                break;
                        }
                    }
                }
            }
        }
    }

    public static Color getRGBADiff(Color sample, Color target)
    {
        float rDiff = Mathf.Abs(target.r - sample.r);
        float gDiff = Mathf.Abs(target.g - sample.g);
        float bDiff = Mathf.Abs(target.b - sample.b);
        float aDiff = Mathf.Abs(target.a - sample.a);

        return new Color(rDiff, gDiff, bDiff, aDiff);
    }
}
