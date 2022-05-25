using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGeneration : MonoBehaviour
{
    // Generates a noise map not sure how scale affects it yet
    public float[,] GenerateNoiseMap(int mapHeight, int mapWidth, float scale)
    {
        //create an empty 2d array to act as the noise map
        //correspaonds to z and x coordinates
        float[,] map = new float[mapHeight, mapWidth];

        for (int z=0; z<mapHeight; z++)
        {
            for (int x=0; x<mapWidth; ++x)
            {
                float sampleX = x / scale;
                float sampleZ = z / scale;

                float noise = Mathf.PerlinNoise(sampleX, sampleZ);

                map[z, x] = noise;
            }
        }
        return map;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
