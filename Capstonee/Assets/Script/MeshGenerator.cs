using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int LOD)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

        int meshSimplificationIncrement = (LOD == 0) ? 1 : LOD * 2;

        int borderedSize = heightMap.GetLength(0);
        int meshSize = borderedSize - 2* meshSimplificationIncrement;
        int meshSizeUnsimplified = borderedSize - 2;

        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

        
        int verticesPerLine = (meshSize -1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine);

        int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
            {
                bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

                if (isBorderVertex)
                {
                    vertexIndicesMap[x,y] = borderVertexIndex;
                    borderedSize--;
                } else
                {
                    vertexIndicesMap[x,y] = meshVertexIndex;
                    meshVertexIndex++;
                }

            }

        }

        for (int y = 0; y < borderedSize; y+= meshSimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x+= meshSimplificationIncrement)
            {
                int vertexIndex = vertexIndicesMap[x,y];
                Vector2 percent = new Vector2((x - meshSimplificationIncrement) / (float)meshSize, (y - meshSimplificationIncrement) / (float)meshSize);
                float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
                Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height , topLeftZ - percent.y * meshSizeUnsimplified);
                
                meshData.AddVertex(vertexPosition, percent, vertexIndex);

                if (x < borderedSize - 1 && y < borderedSize - 1)
                {
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x+meshSimplificationIncrement, y];
                    int c = vertexIndicesMap[x, y + meshSimplificationIncrement];
                    int d = vertexIndicesMap[x +meshSimplificationIncrement, y + meshSimplificationIncrement];

                    meshData.AddTriangle(a, d , c);
                    meshData.AddTriangle(d, a, b);
                }

                vertexIndex++;
            }
        } 
        return meshData;
    }
}

public class MeshData
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    Vector3[] borderVertices;
    int[] borderTriangles;

    int triangleIndex;
    int borderTriangleIndex;
    public MeshData(int VerticesPerLine)
    {
        vertices = new Vector3[VerticesPerLine * VerticesPerLine];
        uvs = new Vector2[VerticesPerLine * VerticesPerLine];
        triangles = new int[(VerticesPerLine-1) * (VerticesPerLine-1)*6];

        borderVertices = new Vector3[VerticesPerLine * 4 + 4];
        borderTriangles = new int[24 * VerticesPerLine];
    }
    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            borderVertices[-vertexIndex-1] = vertexPosition;
        } else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            borderTriangles[borderTriangleIndex] = a;
            borderTriangles[borderTriangleIndex + 1] = b;
            borderTriangles[borderTriangleIndex + 2] = c;
            borderTriangleIndex += 3;
        } else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
        
    }

    Vector3[] CalculatedNormals()
    {
        Vector3[] vertexNormal = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0;  i < triangleCount; i++)
        {
            int normalTriangleINdex = i *3;
            int vertexIndexA = triangles[normalTriangleINdex];
            int vertexIndexB = triangles[normalTriangleINdex + 1];
            int vertexIndexC = triangles[normalTriangleINdex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormal[vertexIndexA] += triangleNormal;
            vertexNormal[vertexIndexB] += triangleNormal;
            vertexNormal[vertexIndexC] += triangleNormal;
        }
        int bordertriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < bordertriangleCount; i++)
        {
            int normalTriangleINdex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleINdex];
            int vertexIndexB = borderTriangles[normalTriangleINdex + 1];
            int vertexIndexC = borderTriangles[normalTriangleINdex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if ( vertexIndexA >= 0)
            {
                vertexNormal[vertexIndexA] += triangleNormal;

            }
            if (vertexIndexB >= 0)
            {
                vertexNormal[vertexIndexB] += triangleNormal;

            }
            if (vertexIndexC >= 0)
            {
                vertexNormal[vertexIndexC] += triangleNormal;

            }
        }

        for (int i = 0; i< vertexNormal.Length; i++)
        {
            vertexNormal[i].Normalize();
        }
        return vertexNormal;
    }
    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = CalculatedNormals();
        return mesh;
    }


}
