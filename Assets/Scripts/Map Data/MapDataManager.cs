using System.Linq;
using System;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataManager : MonoBehaviour {

    public IEnumerable<Vector3> ChunkPositions { get { return chunks.Keys; } }
    public IEnumerable<Chunk> ChunkObjects { get { return chunks.Values; } }

    private List<Vector3> positionsToCheck = new List<Vector3>();
    private Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
    private Vector2 playerPosition;
    private Vector2? currentChunkPosition;
    
    private const int CHUNK_MAX_DISTANCE = 7;
    private const int PROCESSES_PER_FRAME = 3;
    
	private void Update()
    {
        PollInput();

        for (int i = 0; i < PROCESSES_PER_FRAME; i++)
        {
            ProcessInput();
        }        
    }
    private void PollInput()
    {
        playerPosition = Utility.WorldPositionToChunkPosition(Player.Instance.transform.position);
    }
    private void ProcessInput()
    {
        if (!chunks.ContainsKey(playerPosition))
        {
            currentChunkPosition = playerPosition;
        }

        if(!currentChunkPosition.HasValue)
        {
            FindNewChunkPos();
        }
        else if (!chunks.ContainsKey(currentChunkPosition.Value) && IsWithinRange(currentChunkPosition.Value))
        {
            CreateChunk(currentChunkPosition.Value);
        }
        else if (!IsWithinRange(currentChunkPosition.Value))
        {
            DestroyChunk(currentChunkPosition.Value);
        }
        else
        {
            WorkOnNeighbors(currentChunkPosition.Value, pos =>
            {
                if (IsWithinRange(pos) && !chunks.ContainsKey(pos))
                {
                    CreateChunk(pos);

                    positionsToCheck.Add(pos);
                }
                else if (positionsToCheck.Contains(pos))
                {
                    currentChunkPosition = pos;

                    positionsToCheck.Remove(pos);

                    return;
                }
            });

            currentChunkPosition = null;
        }
    }
    private void WorkOnNeighbors(Vector3 anchor, Action<Vector2> callback)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x != 0 && y != 0)
                    continue;

                callback(new Vector2(x + anchor.x, y + anchor.y));
            }
        }
    }
    private bool IsWithinRange(Vector2 position)
    {
        return Vector2.Distance(playerPosition, position) <= CHUNK_MAX_DISTANCE;
    }
    private void FindNewChunkPos()
    {
        if(positionsToCheck.Count > 0)
        {
            currentChunkPosition = positionsToCheck.OrderBy(x => Vector2.Distance(x, playerPosition)).ElementAt(0);

            positionsToCheck.Remove(currentChunkPosition.Value);
        }
        else
        {
            if (chunks.Count <= 0)
            {
                CreateChunk(playerPosition);
            }

            positionsToCheck = new List<Vector3>(ChunkPositions);
        }        
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

        if(currentChunkPosition == chunkPosition)
        {
            currentChunkPosition = null;
        }
    }
}
