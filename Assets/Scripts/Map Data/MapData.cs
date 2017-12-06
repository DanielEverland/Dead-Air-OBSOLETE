using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData {

    public MapData()
    {
        PopulateMap();
    }
        
    public IEnumerable<Chunk> ChunkObjects { get { return _chunks.Values; } }
    public IEnumerable<Vector2> ChunkPositions { get { return _chunks.Keys; } }
    public IDictionary<Vector2, Chunk> Chunks { get { return _chunks; } }

    private Dictionary<Vector2, Chunk> _chunks;

    /// <summary>
    /// How many tiles wide and high should the map be?
    /// </summary>
    private const int MAPSIZE = 512;

    private void PopulateMap()
    {
        _chunks = new Dictionary<Vector2, Chunk>();

        if (((float)MAPSIZE / (float)Chunk.CHUNK_SIZE) % 1 != 0)
        {
            Debug.LogError("Selected mapsize is unvalid. Must be a multiple of chunk size (" + Chunk.CHUNK_SIZE + "). Map size is (" + MAPSIZE + ", " + MAPSIZE + ")");
        }

        int chunkCount = (int)((float)MAPSIZE / (float)Chunk.CHUNK_SIZE);

        for (int x = 0; x < chunkCount; x++)
        {
            for (int y = 0; y < chunkCount; y++)
            {
                Vector2 chunkPosition = new Vector2(x, y);

                Chunk newChunk = new Chunk(chunkPosition);

                _chunks.Add(chunkPosition, newChunk);
            }
        }
    }
}
