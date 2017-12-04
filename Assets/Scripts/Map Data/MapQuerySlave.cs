using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapQuerySlave {

    public Vector2? CurrentChunkPosition { get; set; }
    public List<Vector3> PositionsToCheck { get { return positionsToCheck; } set { positionsToCheck = value; } }
    
    private const int PROCESSES_PER_FRAME = 3;
    
    public event Action OnNoPositionsAvailable;
    public event Func<bool> CustomProcess;
    public event Func<Vector2, bool> CustomNeighborProcess;

    private List<Vector3> positionsToCheck = new List<Vector3>();
    private Vector2 playerPosition { get { return MapDataManager.PlayerPosition; } }

    public void Update()
    {
        for (int i = 0; i < PROCESSES_PER_FRAME; i++)
        {
            ProcessInput();
        }
    }
    private void ProcessInput()
    {
        if (!MapDataManager.ContainsKey(playerPosition))
        {
            CurrentChunkPosition = playerPosition;
        }

        if (!CurrentChunkPosition.HasValue)
        {
            FindNewChunkPos();

            return;
        }

        if (CustomProcess != null)
        {
            if (CustomProcess.Invoke())
            {
                return;
            }
        }

        if(CustomNeighborProcess != null)
        {
            WorkOnNeighbors(CurrentChunkPosition.Value, pos =>
            {
                if(CustomNeighborProcess != null)
                {
                    if (CustomNeighborProcess(pos))
                    {
                        return;
                    }
                }                

                if (positionsToCheck.Contains(pos))
                {
                    CurrentChunkPosition = pos;

                    positionsToCheck.Remove(pos);

                    return;
                }
            });
        }
        
        CurrentChunkPosition = null;
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
    
    private void FindNewChunkPos()
    {
        if (positionsToCheck.Count > 0)
        {
            CurrentChunkPosition = positionsToCheck.OrderBy(x => Vector2.Distance(x, playerPosition)).ElementAt(0);

            positionsToCheck.Remove(CurrentChunkPosition.Value);
        }
        else
        {

            if (OnNoPositionsAvailable != null)
                OnNoPositionsAvailable.Invoke();
            
            positionsToCheck = new List<Vector3>(MapDataManager.ChunkPositions);
        }
    }
}
