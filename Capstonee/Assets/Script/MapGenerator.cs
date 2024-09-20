using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColourMap,
        Mesh,
        FallofMap
    }

    public DrawMode drawMode;

    public Noise.NormalizeMode normalizeMode;

    [Header("Luas Map")]
    public const int mapChunkSize = 239;
    [Range(0,6)]
    public int editorPreviewLOD;
    public float noiseScale;
    
    [Header("Tinggi dan Berapa banyak")]
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    [Header("Random")]
    public int seed;
    public Vector2 offset;

    [Header("Region")]
    public TerrainType[] region;

    [Header("Tinggi Mesh")]
    public float meshHeight;
    public AnimationCurve meshHeightCurve;

    [Header("Falloff (optional)")]
    public bool useFalloff;

    //public GrassGenerator grassGenerator;

    public bool autoUpdate;


    float[,] falloffMap;
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Awake()
    {
        //GrassGenerator grassGenerator = FindObjectOfType<GrassGenerator>();
        falloffMap = FalloffGenerator.GenerateFallofMap(mapChunkSize);
    }

    public void DrawMapEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        
        
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
                break;
            case DrawMode.ColourMap:
                display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
                break;
            case DrawMode.Mesh:
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeight, meshHeightCurve, editorPreviewLOD), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
                break;
            case DrawMode.FallofMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFallofMap(mapChunkSize)));
                break;

        }


        /*if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeight, meshHeightCurve, editorPreviewLOD), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }*/
    }

    public void requestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(centre, callback);
        };
        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData,int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData,lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData,int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeight, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }


    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int  i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if(meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i< meshDataThreadInfoQueue.Count; ++i)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize + 2, mapChunkSize + 2,seed, noiseScale, octaves, persistance, lacunarity, centre + offset, normalizeMode);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize]; 

        for (int y = 0; y<mapChunkSize; y++)
        {
            for (int x = 0; x<mapChunkSize; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x,y]);
                }
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i< region.Length; i++)
                {
                    if (currentHeight >= region[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = region[i].colour;
                    } else
                    {
                        break;
                    }
                }
            }
        }
        
        //grassGenerator.GenerateGrass(noiseMap);


        return new MapData(noiseMap, colourMap);

    }

    private void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }

        falloffMap = FalloffGenerator.GenerateFallofMap(mapChunkSize);
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo (Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }


}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}





