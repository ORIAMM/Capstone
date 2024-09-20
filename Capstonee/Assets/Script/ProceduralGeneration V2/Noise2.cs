using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace ProceduralGeneration {
    public static class Noise2
    {
        public enum NormalizedMode { Local,Global }
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizedMode mode)
        {
            float[,] noisemap = new float[mapWidth, mapHeight];
            float maxPossibleHeight = 0, amplitude = 1, frequency = 1;
            //pseudo-random RNG (Random Number Generator)
            System.Random prng = new(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) - offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= persistance;
            }

            Mathf.Clamp(scale, 0.000001f, scale);

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 0;

                    //makin banyak octave makin abstract / random bentuk gunung
                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency ;
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency ;

                        float perlinvalue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinvalue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }
                    maxNoiseHeight = noiseHeight > maxNoiseHeight ? noiseHeight : maxNoiseHeight;
                    minNoiseHeight = noiseHeight < minNoiseHeight ? noiseHeight : minNoiseHeight;
                    noisemap[x, y] = noiseHeight;
                }
            }
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    //buat dapet nilai (0-1) di noiseMap aslinya jadi bisa dibuat texture
                    if(mode == NormalizedMode.Local){
                        noisemap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noisemap[x, y]);
                    }else if(mode == NormalizedMode.Global) {
                        float NormalizedHeight = (noisemap[x, y] + 1) / (maxPossibleHeight + 1);
                        noisemap[x, y] = Mathf.Clamp(NormalizedHeight, 0, NormalizedHeight);
                    }
                }
            }
            return noisemap;
        }
    }
}