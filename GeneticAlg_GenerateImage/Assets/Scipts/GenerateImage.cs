using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GenerateImage;

public class GenerateImage : MonoBehaviour
{
    public Texture2D target;
    public int popMax;
    public float mutationRate;

    [Range(1,500)]
    public int targetSplitIncrement;

    public int gridWidth;
    public float elementDisplayGap;

    List<SplitTargetInfo> splitTargetInfoList = new List<SplitTargetInfo>();
    List<GameObject> planes = new List<GameObject>();

    SplitTargetInfo testSTI;

    private void Start()
    {
        /*testSTI = new SplitTargetInfo();
        Population testP = new Population(target, popMax, mutationRate);
        testSTI.population = testP;
        testSTI.originalTargetX = 0;
        testSTI.originalTargetY = 0;*/

        for (int targetY = 0; targetY < target.height; targetY+=targetSplitIncrement)
        {
            for(int targetX = 0; targetX < target.width; targetX+=targetSplitIncrement)
            {
                SplitTargetInfo splitTargetInfo = new SplitTargetInfo();
                splitTargetInfo.originalTargetX = targetX;
                splitTargetInfo.originalTargetY = targetY;

                // Sample color array from part of texture
                Texture2D splitTargetTexture = new Texture2D(targetSplitIncrement,targetSplitIncrement);
                Color[] splitTargetColors = new Color[targetSplitIncrement * targetSplitIncrement];
                for (int splitTargetY = 0; splitTargetY < targetSplitIncrement; splitTargetY++)
                {
                    // If target is high enough to be split, traverse x row of target and sample the according pixel
                    if (targetY + splitTargetY < target.height)
                    {
                        for (int splitTargetX = 0; splitTargetX < targetSplitIncrement; splitTargetX++)
                        {
                            // If target is wide enough to be split, get color of associated pixel
                            if (targetX + splitTargetX < target.width)
                                splitTargetColors[splitTargetY * targetSplitIncrement + splitTargetX] = target.GetPixel(targetX, targetY);
                        }
                    }
                }
                splitTargetTexture.SetPixels(splitTargetColors);
                splitTargetTexture.Apply();

                Population splitPopulation = new Population(splitTargetTexture, popMax, mutationRate);

                splitTargetInfo.texture = splitTargetTexture;
                splitTargetInfo.population = splitPopulation;
                splitTargetInfoList.Add(splitTargetInfo);
            }
        }

        GeneratePlanes();
    }

    private void Update()
    {
        // Comment rest for whole target to be target
        /*if (testSTI.population.avgFitness < 0.95)
        {
            UpdateSplitPopulationDisplay(testSTI);
            Draw(testSTI.population);
        }
        else
        {
            Time.timeScale = 0;
        }*/

        // Comment rest for target to be split

        for(int i = 0; i < planes.Count; i++)
        {
            UpdateMaybeBetter(i);
        }

        for(int i = 0; i < splitTargetInfoList.Count; i++)
        {
            Population currentPopulation = splitTargetInfoList[i].population;
            if (currentPopulation.avgFitness < 0.95)
            {
                Draw(currentPopulation);
            }
            else
            {
                Time.timeScale = 0;
            }
        }

        /*for(int i = 0; i < splitTargetInfoList.Count; i++)
        {
            Population currentPopulation = splitTargetInfoList[i].population;
            if (currentPopulation.avgFitness < 0.95)
            {
                UpdateSplitPopulationDisplay(splitTargetInfoList[i]);
                Draw(currentPopulation);
            }
            else
            {
                Time.timeScale = 0;
            }
        }*/
    }

    public void Draw(Population p)
    {
        p.calcFitness();
        //p.naturalSelection();
        p.generate();
    }

    public void GeneratePlanes()
    {
        planes.Clear();
        int height = Mathf.CeilToInt((float)popMax / gridWidth);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (y * gridWidth + x >= popMax)
                {
                    // Exit if grid would generate more cells than elements in population
                    break;
                }
                else
                {
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    obj.transform.parent = this.transform;
                    obj.transform.localScale = new Vector3(target.width, 1, target.height) / 10f;
                    obj.transform.localPosition = new Vector3(x * target.width + x * elementDisplayGap, 0, y * target.height + y * elementDisplayGap);
                    Texture2D defaultTexture = new Texture2D(target.width, target.height);
                    Color[] colorArr = new Color[target.width * target.height];
                    for(int i = 0; i < colorArr.Length; i++)
                    {
                        colorArr[i] = new Color(1,0,0,1);
                    }
                    defaultTexture.SetPixels(colorArr);
                    defaultTexture.filterMode = FilterMode.Point;
                    defaultTexture.wrapMode = TextureWrapMode.Clamp;
                    defaultTexture.Apply();
                    obj.GetComponent<MeshRenderer>().material.mainTexture = defaultTexture;
                    planes.Add(obj);
                }
            }
        }
    }

    public void UpdateSplitPopulationDisplay(SplitTargetInfo splitTargetInfo)
    {
        // Set the pixel part that one DNA element of splitTargetInfo is referencing to that of one plane, for all planes
        for(int i = 0; i < planes.Count; i++)
        {
            Texture2D currentPlaneTexture = (Texture2D)planes[i].GetComponent<MeshRenderer>().material.mainTexture;

            int dataY = 0;
            for(int originalTargetY = splitTargetInfo.originalTargetY; originalTargetY < splitTargetInfo.originalTargetY+targetSplitIncrement; originalTargetY++)
            {
                if( originalTargetY < target.height)
                {
                    // If y position is still within target bounds, traverse x row of target and set the according pixel color
                    int dataX = 0;
                    for (int originalTargetX = splitTargetInfo.originalTargetX; originalTargetX < splitTargetInfo.originalTargetX + targetSplitIncrement; originalTargetX++)
                    {
                        if (originalTargetX < target.width)
                        {
                            // If x position is still within target bounds, set this pixel with the according pixel color
                            Color pixelColor = splitTargetInfo.population.population[i].pixelColors[dataX, dataY];
                            currentPlaneTexture.SetPixel(originalTargetX, originalTargetY, pixelColor);
                            dataX++;
                        }
                    }
                    dataY++;
                }
            }

            currentPlaneTexture.Apply();
            planes[i].GetComponent<MeshRenderer>().material.mainTexture = currentPlaneTexture;
        }
    }

    public void UpdateMaybeBetter(int generation)
    {
        Texture2D planeTexture = (Texture2D)planes[generation].GetComponent<MeshRenderer>().material.mainTexture;

        for(int i = 0; i < splitTargetInfoList.Count; i++)
        {
            SplitTargetInfo curSplitTarget = splitTargetInfoList[i];
            int currentSplitTargetOriginalX = curSplitTarget.originalTargetX;
            int currentSplitTargetOriginalY = curSplitTarget.originalTargetY;
            DNA currentSplitTargetOneGeneration = curSplitTarget.population.population[generation];

            int dataY = 0;
            for (int originalTargetY = currentSplitTargetOriginalY; originalTargetY < currentSplitTargetOriginalY + targetSplitIncrement; originalTargetY++)
            {
                if (originalTargetY < target.height)
                {
                    // If y position is still within target bounds, traverse x row of target and set the according pixel color
                    int dataX = 0;
                    for (int originalTargetX = currentSplitTargetOriginalX; originalTargetX < currentSplitTargetOriginalX + targetSplitIncrement; originalTargetX++)
                    {
                        if (originalTargetX < target.width)
                        {
                            // If x position is still within target bounds, set this pixel with the according pixel color
                            Color pixelColor = curSplitTarget.population.population[generation].pixelColors[dataX, dataY];
                            planeTexture.SetPixel(originalTargetX, originalTargetY, pixelColor);
                            dataX++;
                        }
                    }
                    dataY++;
                }
            }
        }

        planeTexture.Apply();
        planes[generation].GetComponent<MeshRenderer>().material.mainTexture = planeTexture;
    }

    public struct SplitTargetInfo
    {
        public Texture2D texture;
        public Population population;
        public int originalTargetX;
        public int originalTargetY;
    }
}
