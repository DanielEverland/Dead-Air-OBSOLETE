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
                terrainTiles[x, y] = (byte)Mathf.Round((TileType.AllTypes.Count - 1) * GetPerlinValue(x, y));
            }
        }
    }

    public const int PIXELS_PER_TILE = 32;
    public const int CHUNK_SIZE = 32;

    public bool HasGameObject { get { return gameObject != null; } }
    public byte?[,] WallTiles { get { return wallTiles; } }
    public byte[,] TerrainTiles { get { return terrainTiles; } }
    public Vector3 Position { get { return chunkOffset; } }
    public GameObject GameObject { get { return gameObject; } }

    private readonly Vector3 chunkOffset;

    private byte?[,] wallTiles = new byte?[CHUNK_SIZE, CHUNK_SIZE];
    private byte[,] terrainTiles = new byte[CHUNK_SIZE, CHUNK_SIZE];
    private GameObject gameObject;
    
    public static bool IsValidLocalPosition(Vector3 position)
    {
        return position.x >= 0 && position.y >= 0
            &&
            position.x < CHUNK_SIZE && position.y < CHUNK_SIZE;
    }
    public byte GetChunkTile(byte x, byte y)
    {
        if (wallTiles[x, y].HasValue)
            return wallTiles[x, y].Value;

        return terrainTiles[x, y];
    }
    public void RemoveWallTile(Vector2 localPosition)
    {
        if (!IsValidLocalPosition(localPosition))
            throw new System.IndexOutOfRangeException("Local position " + localPosition + " is out of bounds. Chunk size is " + CHUNK_SIZE);

        byte x = (byte)Mathf.FloorToInt(localPosition.x);
        byte y = (byte)Mathf.FloorToInt(localPosition.y);
        byte? currentValue = wallTiles[x, y];

        if (currentValue.HasValue)
        {
            wallTiles[x, y] = null;
            SetDirty();
        }         
    }
    public void SetWallTile(byte tileIndex, Vector2 localPosition)
    {
        TileType tile = TileType.AllTypes[tileIndex];

        if (!(tile is WallType))
            throw new System.InvalidCastException("Tile " + tile + " isn't Wall Type");

        SetWallTile(tile as WallType, localPosition);
    }
    public void SetWallTile(WallType type, Vector2 localPosition)
    {
        if (!IsValidLocalPosition(localPosition))
            throw new System.IndexOutOfRangeException("Local position " + localPosition + " is out of bounds. Chunk size is " + CHUNK_SIZE);

        byte x = (byte)Mathf.FloorToInt(localPosition.x);
        byte y = (byte)Mathf.FloorToInt(localPosition.y);
        byte? currentValue = wallTiles[x, y];

        if (!currentValue.HasValue)
        {
            wallTiles[x, y] = type.ID;
            SetDirty();
        }
        else if(currentValue.Value != type.ID)
        {
            wallTiles[x, y] = type.ID;
            SetDirty();
        }
    }
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
    public void SetDirty()
    {
        MapData.SetDirty(this);
    }
}
