using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProceduralGeneration
{
    public class MapGenerator2 : MonoBehaviour
    {
        public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap}

        public DrawMode drawMode;
        
        public const int MAPCHUNKSIZE = 239;
        [Header("Map Generation Settings")]
        public Noise2.NormalizedMode normalizedMode;
        [Tooltip("Number of Octaves / Noise Instances for the map generation")]
        public int octaves;

        [Tooltip("Multiplier for the Amplitude (Lebih kecil, lebih rata amplitudonya tiap oktaf)")]
        [Range(0f, 1f)]
        public float persistance;

        [Tooltip("Multiplier for the frequency (lebih besar, lebih banyak bentukan gelombang yang dibuat per oktaf)")]
        public float lacunarity;
        [Space(10)]
        public int seed;
        public float noiseScale;
        public Vector2 offset;
        [Space(20)]
        public bool AutoUpdate;
        [Header("Colorisation Settings")]
        public List<TerrainType> Regions;

        [Header("Mesh Settings\n======================")]
        public float heightMultiplier;
        public AnimationCurve meshHeightCurve;
        [Range(0, 6)]
        public int EditorPreview_LevelOfDetail;

        [Header("Falloff Settings\n======================")]
        public bool useFalloff;
        public float FallOffOffset;
        public float FallOffMultiplier;

        float[,] falloffMap;

        Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new();
        Queue<MapThreadInfo<MeshData2>> meshDataThreadInfoQueue = new();
        private void Awake()
        {
            falloffMap = FallofGenerator.GenerateFallofMap(MAPCHUNKSIZE, FallOffOffset, FallOffMultiplier);
        }

        public void DrawMapInEditor()
        {
            MapData mapData = GenerateMapData(Vector2.zero);
            MapDisplay2 display = FindObjectOfType<MapDisplay2>();
            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    display.DrawTexture(TextureGenerator2.TextureFromHeightMap(mapData.heightMap ));
                    break;
                case DrawMode.ColorMap:
                    display.DrawTexture(TextureGenerator2.TextureFromColourMap(mapData.colourMap, MAPCHUNKSIZE, MAPCHUNKSIZE));
                    break;
                case DrawMode.Mesh:
                    display.DrawMesh(MeshGenerator2.GenerateTerrainMesh(mapData.heightMap, heightMultiplier, meshHeightCurve, EditorPreview_LevelOfDetail), TextureGenerator.TextureFromColourMap(mapData.colourMap, MAPCHUNKSIZE, MAPCHUNKSIZE));
                    break;
                case DrawMode.FalloffMap:
                    display.DrawTexture(TextureGenerator2.TextureFromHeightMap(FallofGenerator.GenerateFallofMap(MAPCHUNKSIZE, FallOffOffset, FallOffMultiplier)));
                    break;
            }
        }
        public void RequestMapData(Vector2 centre, Action<MapData> callback)
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
                mapDataThreadInfoQueue.Enqueue(new(callback, mapData));
            }
        }
        public void RequestMeshData(MapData mapData, int lod, Action<MeshData2> callback)
        {
            ThreadStart threadStart = delegate
            {
                MeshDataThread(mapData, lod, callback);
            };
            new Thread(threadStart).Start();
        }
        void MeshDataThread(MapData mapData, int lod, Action<MeshData2> callback)
        {
            MeshData2 meshData = MeshGenerator2.GenerateTerrainMesh(mapData.heightMap, heightMultiplier, meshHeightCurve, lod);
            lock(meshDataThreadInfoQueue)
            {
                meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData2>(callback, meshData));
            }
        }
        private void Update()
        {
            if(mapDataThreadInfoQueue.Count > 0)
            {
                for(int i = 0; i < mapDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
            if (meshDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MeshData2> threadInfo = meshDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }
        MapData GenerateMapData(Vector2 centre)
        {
            float[,] noiseMap = Noise2.GenerateNoiseMap(MAPCHUNKSIZE + 2, MAPCHUNKSIZE + 2, seed, noiseScale, octaves, persistance, lacunarity, centre + offset, normalizedMode);

            Color[] colorMap = new Color[MAPCHUNKSIZE * MAPCHUNKSIZE];
            for (int y = 0; y < MAPCHUNKSIZE; y++){
                for (int x = 0; x < MAPCHUNKSIZE; x++){
                    if (useFalloff) noiseMap[x, y] -= Mathf.Clamp01(falloffMap[x, y]);
                    float currentHeight = noiseMap[x, y];
                    foreach (var region in Regions){
                        if (currentHeight <= region.height)
                        {
                            colorMap[y * MAPCHUNKSIZE + x] = region.colour;
                            break;
                        }
                    }
                }
            }
            return new(noiseMap, colorMap);
        }
        void OnValidate()
        {
            Mathf.Clamp(lacunarity, 1, lacunarity);
            Mathf.Clamp(octaves, 1, octaves);
            falloffMap = FallofGenerator.GenerateFallofMap(MAPCHUNKSIZE, FallOffOffset, FallOffMultiplier);
        }
        struct MapThreadInfo<T>
        {
            public Action<T> callback;
            public T parameter;

            public MapThreadInfo(Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }
}

