using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float MaxNoiseHeight = float.MinValue;
        float MinNoiseHeight = float.MaxValue;

        for (int y= 0; y < mapHeight; y++)
        {
            for (int x= 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0;i < octaves; i++)
                {
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)  * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > MaxNoiseHeight)
                {
                    MaxNoiseHeight = noiseHeight;
                } else if (noiseHeight < MinNoiseHeight)
                {
                    MinNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;
            }
        }
        for (int y= 0; y < mapHeight; y++)
        {
            for (int x= 0; x < mapWidth; x++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(MinNoiseHeight, MaxNoiseHeight, noiseMap[x,y]);
            }
        }

        return noiseMap;

    }
}
