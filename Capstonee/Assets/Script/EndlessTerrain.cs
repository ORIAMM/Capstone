using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float ViewerMoveThresholdForUpdate = 25f;
    const float sqrViewerMoveThresholdForUpdate = ViewerMoveThresholdForUpdate * ViewerMoveThresholdForUpdate;
    const float scale = 5f;

    public LODinfo[] detailLevels;
    public static float maxViewDst;

    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPos;
    Vector2 viewerPositionOld;

    static MapGenerator mapGenerator;
    int ChunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDick = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainVisibleLast = new List<TerrainChunk>();

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDsstThreshold;
        ChunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / ChunkSize);

        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z) / scale;

        if ((viewerPositionOld-viewerPos).sqrMagnitude > sqrViewerMoveThresholdForUpdate)
        {
            viewerPositionOld = viewerPos;
            UpdateVisibleChunks();
        }
        
    }

    void UpdateVisibleChunks()
    {
        for ( int i=0; i < terrainVisibleLast.Count; i++)
        {
            terrainVisibleLast[i].SetVisible(false);
        }
        terrainVisibleLast.Clear();

        int currentChunksCordX = Mathf.RoundToInt(viewerPos.x/ChunkSize);
        int currentChunksCordY = Mathf.RoundToInt(viewerPos.y/ChunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 ViewedChunkCord = new Vector2(currentChunksCordX + xOffset, currentChunksCordY + yOffset);

                if (terrainChunkDick.ContainsKey(ViewedChunkCord))
                {
                    terrainChunkDick[ViewedChunkCord].UpdateTerrainChunks();
                    
                } else
                {
                    terrainChunkDick.Add(ViewedChunkCord, new TerrainChunk(ViewedChunkCord, ChunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }

    }

    public class TerrainChunk
    {
        GameObject meshObj;
        Vector2 pos;

        Bounds bounds;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LODinfo[] detailLevels;
        LODMesh[] lodMeshes;

        MapData mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODinfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;
            pos = coord * size;
            bounds = new Bounds(pos, Vector2.one * size);
            Vector3 posV3 = new Vector3(pos.x, 0, pos.y);

            meshObj = new GameObject("TerrainChunks");
            meshRenderer = meshObj.AddComponent<MeshRenderer>();
            meshFilter = meshObj.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObj.transform.position = posV3 * scale;
            meshObj.transform.parent = parent;
            meshObj.transform.localScale = Vector3.one * scale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunks);
            }

            mapGenerator.requestMapData(pos,OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            Debug.Log("WAHYU GANTENGS");
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunks();
        }

        public void UpdateTerrainChunks()
        {
            if (mapDataReceived)
            {
                float ViewDstEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
                bool Visible = ViewDstEdge <= maxViewDst;

                if (Visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (ViewDstEdge > detailLevels[i].visibleDsstThreshold)
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
                        LODMesh lODMesh = lodMeshes[lodIndex];
                        if (lODMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lODMesh.mesh;
                        }
                        else if (!lODMesh.hasRequstedMesh)
                        {
                            lODMesh.RequestMesh(mapData);
                        }
                    }
                    terrainVisibleLast.Add(this);
                }

                SetVisible(Visible);
            }
        }
        public void SetVisible(bool Visible)
        {
            meshObj.SetActive(Visible);
        }

        public bool isVisible()
        {
            return meshObj.activeSelf;
        }


    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequstedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshdata)
        {
            mesh = meshdata.CreateMesh();
            hasMesh = true;

            updateCallback();
        }
        public void RequestMesh(MapData mapData)
        {
            hasRequstedMesh = true;
            mapGenerator.RequestMeshData(mapData,lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODinfo
    {
        public int lod;
        public float visibleDsstThreshold;

    }


}
