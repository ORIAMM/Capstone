using UnityEngine;

public static class MeshGenerator2
{
    public static MeshData2 GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int LevelOfDetail)
    {
        AnimationCurve heightCurve = new(_heightCurve.keys);
        int meshSimplificationIncrement = LevelOfDetail == 0 ? 1 : LevelOfDetail * 2;

        int BorderedSize = heightMap.GetLength(0);
        int meshSize = BorderedSize - 2 * meshSimplificationIncrement;
        int meshSizeUnsimplified = BorderedSize - 2;

        float topLeft_x = (meshSizeUnsimplified - 1) / -2f;
        float topLeft_z = (meshSizeUnsimplified - 1) / 2f;

        int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

        MeshData2 meshData = new (verticesPerLine);
        int[,] VertexIndicesMap = new int[BorderedSize, BorderedSize];
        int MeshVertexIndex = 0;
        int BorderedVertexIndex = -1;
        for (int y = 0; y < BorderedSize; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < BorderedSize; x += meshSimplificationIncrement)
            {
                bool isBorderVertex = y == 0 || (y == BorderedSize - 1) || x == 0 || (x == BorderedSize - 1);
                VertexIndicesMap[x, y] = isBorderVertex ? BorderedVertexIndex-- : MeshVertexIndex++;
            }
        }
            /*
            * A == B
            * ||\ ||
            * || \||
            * C == D
            */
        for (int y = 0; y < BorderedSize; y+= meshSimplificationIncrement)
        {
            for (int x = 0; x < BorderedSize; x+= meshSimplificationIncrement)
            {
                int vertexIndex = VertexIndicesMap[x, y];
                Vector2 Percent = new Vector2((x - meshSimplificationIncrement)/ (float)meshSize, (y - meshSimplificationIncrement)/ (float)meshSize);
                float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
                Vector3 VertexPosition = new Vector3(topLeft_x + Percent.x * meshSizeUnsimplified, height , topLeft_z - Percent.y * meshSizeUnsimplified);

                meshData.AddVertex(VertexPosition, Percent, vertexIndex);
                if (x < BorderedSize - 1 && y < BorderedSize - 1)
                {
                    int a = VertexIndicesMap[x, y];
                    int b = VertexIndicesMap[x + meshSimplificationIncrement, y];
                    int c = VertexIndicesMap[x, y + meshSimplificationIncrement];
                    int d = VertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];

                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
                }
                vertexIndex++;
            }
        }
        return meshData;
    }
}
public class MeshData2
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    //Vertices that will not be included in the array
    Vector3[] borderVertices;
    int[] borderTriangles;

    int triangleIndex;
    int borderTriangleIndex;
    public MeshData2(int VerticesPerLine)
    {
        vertices = new Vector3[VerticesPerLine * VerticesPerLine];
        uvs = new Vector2[VerticesPerLine * VerticesPerLine];
        triangles = new int[((VerticesPerLine - 1) * (VerticesPerLine) - 1) * 6];

        borderVertices = new Vector3[VerticesPerLine * 4 + 4];
        borderTriangles = new int[24 * VerticesPerLine];
    }
    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if(vertexIndex < 0){
            borderVertices[-vertexIndex - 1] = vertexPosition;
        }
        else{
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }
    public void AddTriangle(int a, int b, int c)
    {
        if(a < 0 || b < 0 || c < 0)
        {
            borderTriangles[borderTriangleIndex++] = a;
            borderTriangles[borderTriangleIndex++] = b;
            borderTriangles[borderTriangleIndex++] = c;
        }
        else
        {
            triangles[triangleIndex++] = a;
            triangles[triangleIndex++] = b;
            triangles[triangleIndex++] = c;
        }

    }
    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length/3;
        for(int i = 0; i < triangleCount; i++){
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex++];
            int vertexIndexB = triangles[normalTriangleIndex++];
            int vertexIndexC = triangles[normalTriangleIndex];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }
        int borderTriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleIndex++];
            int vertexIndexB = borderTriangles[normalTriangleIndex++];
            int vertexIndexC = borderTriangles[normalTriangleIndex];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if (vertexIndexA >= 0) vertexNormals[vertexIndexA] += triangleNormal;
            if (vertexIndexB >= 0) vertexNormals[vertexIndexB] += triangleNormal;
            if (vertexIndexC >= 0) vertexNormals[vertexIndexC] += triangleNormal;
        }
        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }
        return vertexNormals;
    }
    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC){
        Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }
    public Mesh CreateMesh()
    {
        Mesh mesh = new()
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.normals = CalculateNormals();
        //mesh.RecalculateNormals();
        return mesh;
    }
}

/*
    animator.setfloat(rb.velocity.magnitude); //speed
 */
