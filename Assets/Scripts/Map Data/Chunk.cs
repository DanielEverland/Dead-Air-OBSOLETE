using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

    public Chunk()
    {
        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                tiles[x, y] = (byte)Random.Range(0, TileType.AllTypes.Count - 1);
            }
        }
    }

    public byte[,] Tiles { get { return tiles; } }

    private byte[,] tiles = new byte[CHUNK_SIZE, CHUNK_SIZE];

    public const int PIXELS_PER_TILE = 32;
    public const int CHUNK_SIZE = 32;
}
