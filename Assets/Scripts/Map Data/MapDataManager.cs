using System.Linq;
using System;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataManager : MonoBehaviour {

    public static IEnumerable<Vector3> ChunkPositions { get { return chunks.Keys; } }
    public static IEnumerable<Chunk> ChunkObjects { get { return chunks.Values; } }
    public static Vector2 PlayerPosition { get; set; }

    private static Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();

    private static MapQuerySlave CreateChunkSlave = new MapQuerySlave();
    private static MapQuerySlave DestroyChunkSlave = new MapQuerySlave();

    private const int CHUNK_MAX_DISTANCE = 10;

    private void Awake()
    {
        CreateChunkSlave.OnNoPositionsAvailable += () =>
        {
            if (chunks.Count <= 0)
            {
                CreateChunk(PlayerPosition);
            }
        };

        CreateChunkSlave.CustomNeighborProcess += (Vector2 pos) =>
        {
            if (IsWithinRange(pos) && !ContainsKey(pos))
            {
                CreateChunk(pos);

                CreateChunkSlave.PositionsToCheck.Add(pos);

                return true;
            }

            return false;
        };

        CreateChunkSlave.CustomProcess += () =>
        {
            if (!ContainsKey(CreateChunkSlave.CurrentChunkPosition.Value) && IsWithinRange(CreateChunkSlave.CurrentChunkPosition.Value))
            {
                CreateChunk(CreateChunkSlave.CurrentChunkPosition.Value);
            }

            return false;
        };

        DestroyChunkSlave.CustomProcess += () =>
        {
            if (!IsWithinRange(DestroyChunkSlave.CurrentChunkPosition.Value))
            {
                DestroyChunk(DestroyChunkSlave.CurrentChunkPosition.Value);
            }

            return false;
        };
    }
    private void Update()
    {
        PlayerPosition = Utility.WorldPositionToChunkPosition(Player.Instance.transform.position);

        CreateChunkSlave.Update();
        DestroyChunkSlave.Update();
    }
    public static bool ContainsKey(Vector3 key)
    {
        return chunks.ContainsKey(key);
    }
    private bool IsWithinRange(Vector2 position)
    {
        return Vector2.Distance(PlayerPosition, position) <= CHUNK_MAX_DISTANCE;
    }
    private void CreateChunk(Vector3 chunkPosition)
    {
        Chunk newChunk = new Chunk(chunkPosition);

        chunks.Add(chunkPosition, newChunk);
        ChunkGenerator.RenderChunk(newChunk);
    }
    private void DestroyChunk(Vector3 chunkPosition)
    {
        Chunk chunk = chunks[chunkPosition];
        chunks.Remove(chunkPosition);

        Destroy(chunk.GameObject);

        if(DestroyChunkSlave.CurrentChunkPosition == chunkPosition)
        {
            DestroyChunkSlave.CurrentChunkPosition = null;
        }
    }

}
