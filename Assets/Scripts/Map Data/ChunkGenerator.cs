using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator {

    private static int submeshID;
    private static int verticeCount;
    private static MeshFilter currentMeshFilter;
    private static Mesh currentMesh;
    private static Vector3[] vertices = new Vector3[(Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE) * 4];
    private static List<List<int>> triangles = new List<List<int>>();
    private static List<Material> materials = new List<Material>();
    private static Vector3[] Normals
    {
        get
        {
            if (normals == null)
                CreateNormals();

            return normals;
        }
    }
    private static Vector3[] normals;

    public static void Initialize()
    {
        CreateNormals();
    }
	public static void RenderChunk(Chunk chunk)
    {
        if (chunk.HasGameObject)
        {
            currentMeshFilter = chunk.GameObject.GetComponent<MeshFilter>();
        }
        else
        {
            currentMeshFilter = ChunkObjectPool.GetMeshFilter();
            chunk.AssignGameObject(currentMeshFilter.gameObject);
        }
        
        currentMesh = new Mesh();
        verticeCount = 0;
        materials.Clear();
        triangles.Clear();
        
        for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
            {
                AddTile(x, y, chunk.GetChunkTile((byte)x, (byte)y));
            }
        }

        currentMesh.vertices = vertices;
        currentMesh.subMeshCount = triangles.Count;

        for (int i = 0; i < triangles.Count; i++)
        {
            currentMesh.SetTriangles(triangles[i], i);
        }

        currentMesh.normals = normals;

        currentMeshFilter.mesh = currentMesh;
        currentMeshFilter.GetComponent<MeshRenderer>().materials = materials.ToArray();
        currentMeshFilter.gameObject.transform.position = new Vector3(Chunk.CHUNK_SIZE * chunk.Position.x, Chunk.CHUNK_SIZE * chunk.Position.y);
    }
    private static void AddTile(int x, int y, byte tileType)
    {
        if (materials.Contains(TileType.AllTypes[tileType].Material))
        {
            submeshID = materials.IndexOf(TileType.AllTypes[tileType].Material);
        }
        else
        {
            submeshID = materials.Count;
            materials.Add(TileType.AllTypes[tileType].Material);
            triangles.Add(new List<int>());
        }

        vertices[verticeCount + 0] = new Vector3(1 + x, 1 + y);
        vertices[verticeCount + 1] = new Vector3(1 + x, 0 + y);
        vertices[verticeCount + 2] = new Vector3(0 + x, 1 + y);
        vertices[verticeCount + 3] = new Vector3(0 + x, 0 + y);
        
        triangles[submeshID].AddRange(new List<int>(6)
        {
            3 + verticeCount, 0 + verticeCount, 1 + verticeCount,
            2 + verticeCount, 0 + verticeCount, 3 + verticeCount,
        });

        verticeCount += 4;
    }
    private static void CreateNormals()
    {
        normals = new Vector3[(Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE) * 4];

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.back;
        }
    }
}
