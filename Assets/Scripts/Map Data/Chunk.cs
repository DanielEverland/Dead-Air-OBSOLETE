using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

    private const float NOISE_ZOOM = 20;

    private Chunk() { }
    public Chunk(Vector3 chunkOffset)
    {
        this.chunkOffset = chunkOffset;
        this.chunkOffset.z = 0;
                        
        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                tiles[x, y] = (byte)Mathf.Round((TileType.AllTypes.Count - 1) * GetPerlinValue(x, y));
            }
        }
    }

    public const int PIXELS_PER_TILE = 32;
    public const int CHUNK_SIZE = 32;
    
    public byte[,] Tiles { get { return tiles; } }
    public Vector3 Position { get { return chunkOffset; } }
    public GameObject GameObject { get { return gameObject; } }

    private readonly Vector3 chunkOffset;

    private byte[,] tiles = new byte[CHUNK_SIZE, CHUNK_SIZE];
    private GameObject gameObject;
    
    private float GetPerlinValue(int x, int y)
    {
        return Mathf.PerlinNoise(
            (float)(x + chunkOffset.x * Chunk.CHUNK_SIZE) / (float)MapData.MAPSIZE * NOISE_ZOOM,
            (float)(y + chunkOffset.y * Chunk.CHUNK_SIZE) / (float)MapData.MAPSIZE * NOISE_ZOOM);
    }
    public void AssignGameObject(GameObject obj)
    {
        if(gameObject != null)
        {
            throw new System.Exception("A gameobject has already been assigned to this chunk");
        }

        gameObject = obj;
    }
}
