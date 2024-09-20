using ProceduralGeneration;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndlessTerain2 : MonoBehaviour
{
    const float scale = 10f;

    const float viewerMoveTresholdForChunkUpdate = 25f;
    const float sqrViewerMoveTresholdForChunkUpdate = viewerMoveTresholdForChunkUpdate * viewerMoveTresholdForChunkUpdate;

    public List<LODInfo> DetailLevels;
    public static float MaxViewDistance = 450;

    public Transform viewer;

    public static Vector2 viewerPosition;
    Vector2 viewerpositionold;
    public Material mapMaterial;

    static MapGenerator2 mapGenerator;
    int chunkSize;
    int chunksVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> TerrainChunkDictionary = new();
    static List<TerrainChunk> visibleTerrainChunkLastUpdate;
    private void Start()
    {
        visibleTerrainChunkLastUpdate = new();
        mapGenerator = FindObjectOfType<MapGenerator2>();
        chunkSize = MapGenerator2.MAPCHUNKSIZE - 1;

        MaxViewDistance = DetailLevels[DetailLevels.Count - 1].visibleDistanceTreshold;
        chunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance/chunkSize);
        Debug.Log(visibleTerrainChunkLastUpdate.Count);
        UpdateVisibleChunks();
    }
    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;

        if((viewerpositionold - viewerPosition).sqrMagnitude > sqrViewerMoveTresholdForChunkUpdate)
        {
            viewerpositionold = viewerPosition;
            UpdateVisibleChunks();
        }
    }
    void UpdateVisibleChunks()
    {
        foreach(TerrainChunk chunk in visibleTerrainChunkLastUpdate)
        {
            chunk.SetVisible(false);
        }
        visibleTerrainChunkLastUpdate.Clear();
        int CurrentChunkCordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int CurrentChunkCordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for(int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCord = new(CurrentChunkCordX + xOffset, CurrentChunkCordY + yOffset);
                if (TerrainChunkDictionary.ContainsKey(viewedChunkCord))
                {
                    TerrainChunkDictionary[viewedChunkCord].UpdateTerrainChunk();
                }
                else
                {
                    TerrainChunkDictionary.Add(viewedChunkCord, new TerrainChunk(viewedChunkCord, chunkSize, DetailLevels,transform, mapMaterial));
                }
            }
        }

    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        MeshCollider meshCollider;

        LODInfo[] detailLevels;
        LODMesh[] LODmeshes;

        MapData mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;
        public TerrainChunk(Vector2 coordinate, int size, List<LODInfo> detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels.ToArray();

            position = coordinate * size;
            bounds = new(position, Vector2.one * size);
            Vector3 positionV3 = new(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = new Vector3(scale,1,scale);
            SetVisible(false);

            LODmeshes = new LODMesh[detailLevels.Count];
            for(int i = 0; i < LODmeshes.Length; i++)
            {
                LODmeshes[i] = new(detailLevels[i].lod, UpdateTerrainChunk);
            }
            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }
        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator2.MAPCHUNKSIZE, MapGenerator2.MAPCHUNKSIZE);
            meshRenderer.material.mainTexture = texture;
            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewerDistance = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistance <= MaxViewDistance;

                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDistance > detailLevels[i].visibleDistanceTreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = LODmeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                            meshCollider.sharedMesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                    visibleTerrainChunkLastUpdate.Add(this);
                }
                SetVisible(visible);
            }
        }
        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }
        public bool isVisible => meshObject.activeSelf;
    }
    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int LOD;
        Action updateCallback;
        public LODMesh(int lod, Action updatecallback)
        {
            LOD = lod;
            updateCallback = updatecallback;
        }
        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, LOD, OnMeshDataReceived);
        }
        public void OnMeshDataReceived(MeshData2 meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }
    }
    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDistanceTreshold;
    }
}
