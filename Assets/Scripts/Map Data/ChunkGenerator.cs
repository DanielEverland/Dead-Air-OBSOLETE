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
    private static List<Vector3> vertices = new List<Vector3>();
    private static List<List<int>> triangles = new List<List<int>>();
    private static List<Material> materials = new List<Material>();
    private static Texture2D chunkTexture;
    private static TileType currentTile;
    private static Material currentMaterial;

    static System.Diagnostics.Stopwatch stopWatch;

	public static void RenderChunk(Chunk chunk)
    {
        stopWatch = new System.Diagnostics.Stopwatch();


        RunTest(RenderChunkUsingSpriteRenderer, "Sprite");
        RunTest(RenderChunkUsingFastMeshRendering, "Fast Mesh");
        RunTest(RenderChunkUsingMeshRenderer, "Mesh");
    }
    private static void RunTest(Action<Chunk> callback, string type)
    {
        List<long> milliseconds = new List<long>();

        for (int i = 0; i < 100; i++)
        {
            Chunk newChunk = new Chunk();

            stopWatch.Reset();
            stopWatch.Start();

            callback(new Chunk());

            stopWatch.Stop();
            milliseconds.Add(stopWatch.ElapsedMilliseconds);
        }

        Debug.Log(type + " took " + Mathf.RoundToInt((float)milliseconds.Average()) + " milliseconds");
    }
    private static void RenderChunkUsingFastMeshRendering(Chunk chunk)
    {
        currentMeshFilter = ChunkObjectPool.GetMeshFilter();
        currentMesh = new Mesh();
        chunkTexture = new Texture2D(Chunk.PIXELS_PER_TILE * Chunk.CHUNK_SIZE, Chunk.PIXELS_PER_TILE * Chunk.CHUNK_SIZE);

        for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
            {
                currentTile = TileType.AllTypes[chunk.Tiles[x, y]];

                chunkTexture.SetPixels(x * Chunk.PIXELS_PER_TILE, y * Chunk.PIXELS_PER_TILE, Chunk.PIXELS_PER_TILE, Chunk.PIXELS_PER_TILE, currentTile.Texture.GetPixels());
            }
        }

        chunkTexture.Apply();
        currentMaterial = new Material(Shader.Find("Standard"));
        currentMaterial.mainTexture = chunkTexture;

        currentMesh.vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(Chunk.CHUNK_SIZE, 0, 0),
            new Vector3(Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE, 0),
            new Vector3(0, Chunk.CHUNK_SIZE, 0),
        };

        currentMesh.triangles = new int[6]
        {
            3, 0, 1,
            2, 0, 3,
        };

        currentMesh.RecalculateNormals();
        currentMeshFilter.mesh = currentMesh;
        currentMeshFilter.GetComponent<MeshRenderer>().material = currentMaterial;
    }
    private static void RenderChunkUsingMeshRenderer(Chunk chunk)
    {
        currentMeshFilter = ChunkObjectPool.GetMeshFilter();
        currentMesh = new Mesh();
        verticeCount = 0;
        materials.Clear();
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
            {
                AddTile(x, y, chunk.Tiles[x, y]);
            }
        }

        currentMesh.vertices = vertices.ToArray();
        currentMesh.subMeshCount = triangles.Count;

        for (int i = 0; i < triangles.Count; i++)
        {
            currentMesh.SetTriangles(triangles[i], i);
        }
        
        currentMesh.RecalculateNormals();

        currentMeshFilter.mesh = currentMesh;
        currentMeshFilter.GetComponent<MeshRenderer>().materials = materials.ToArray();
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

        vertices.AddRange(new List<Vector3>(4)
        {
            new Vector3(0.5f    + x, 0.5f   + y),
            new Vector3(0.5f    + x, -0.5f  + y),
            new Vector3(-0.5f   + x, 0.5f   + y),
            new Vector3(-0.5f   + x, -0.5f  + y),
        });

        triangles[submeshID].AddRange(new List<int>(6)
        {
            3 + verticeCount, 0 + verticeCount, 1 + verticeCount,
            2 + verticeCount, 0 + verticeCount, 3 + verticeCount,
        });

        verticeCount += 4;
    }
    private static void RenderChunkUsingSpriteRenderer(Chunk chunk)
    {
        chunkTexture = new Texture2D(Chunk.PIXELS_PER_TILE * Chunk.CHUNK_SIZE, Chunk.PIXELS_PER_TILE * Chunk.CHUNK_SIZE);

        for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
            {
                currentTile = TileType.AllTypes[chunk.Tiles[x, y]];

                chunkTexture.SetPixels(x * Chunk.PIXELS_PER_TILE, y * Chunk.PIXELS_PER_TILE, Chunk.PIXELS_PER_TILE, Chunk.PIXELS_PER_TILE, currentTile.Texture.GetPixels());
            }
        }

        chunkTexture.Apply();

        ChunkObjectPool.GetSpriteRenderer().sprite = Sprite.Create(chunkTexture, new Rect(0, 0, chunkTexture.width, chunkTexture.height), Vector2.one / 2);
    }
}
