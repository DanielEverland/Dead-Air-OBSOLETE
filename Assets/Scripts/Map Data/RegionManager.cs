using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RegionManager {

    private static Dictionary<Vector2, Region> _regions;

    public static void Initialize()
    {
        ExecuteFullRegeneration();
    }
    private static void ExecuteFullRegeneration()
    {
        _regions = new Dictionary<Vector2, Region>();
        
        Queue<Vector2> remainingPositions = new Queue<Vector2>();

        for (int x = 0; x < MapData.MAPSIZE; x++)
        {
            for (int y = 0; y < MapData.MAPSIZE; y++)
            {
                remainingPositions.Enqueue(new Vector2(x, y));
            }
        }

        //Performance optimization. Since we're traversing the list in order of insertion, it's 
        //probable that positions will be inserted into the most-recently created region
        Region newestRegion = Create(Vector2.zero);
        Add(newestRegion);
                        
        while (remainingPositions.Count > 0)
        {
            Vector2 currentPosition = remainingPositions.Dequeue();
            
            if (!newestRegion.Contains(currentPosition))
            {
                if (!Exists(currentPosition))
                {
                    Add(Create(currentPosition));
                }
            }
        }
    }
    private static void Add(Region region)
    {
        _regions.Add(region.Position, region);   
    }
    private static Region Create(Vector2 position)
    {
        Queue<Vector2> toCheck = new Queue<Vector2>();
        HashSet<Vector2> blacklist = new HashSet<Vector2>();
        Vector2 size = Vector2.zero;

        toCheck.Enqueue(position);

        while (toCheck.Count > 0)
        {
            Vector2 currentPosition = toCheck.Dequeue();
            blacklist.Add(currentPosition);

            Vector2 delta = currentPosition - position;

            size.x = Mathf.Max(delta.x, size.x);
            size.y = Mathf.Max(delta.y, size.y);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if ((x != 0 && y != 0) || (x == 0 && y == 0))
                        continue;

                    Vector2 loopPosition = currentPosition + new Vector2(x, y);
                    Vector2 loopDelta = loopPosition - position;

                    if (MapData.IsValidPosition(loopPosition) && MapData.IsPassable(loopPosition) && !blacklist.Contains(loopPosition) &&
                        loopDelta.x < Region.SIZE && loopDelta.y < Region.SIZE && loopDelta.x > 0 && loopDelta.y > 0)
                        toCheck.Enqueue(loopPosition);
                }
            }
        }
        
        return new Region(position, size);
    }
    public static bool Exists(Vector2 position)
    {
        Vector2 anchor = Utility.WorldPositionToRegionPosition(position);
        Vector2 currentPosition = anchor;

        for (int x = 0; x < Region.SIZE; x++)
        {
            for (int y = 0; y < Region.SIZE; y++)
            {
                if (_regions.ContainsKey(currentPosition))
                {
                    Region currentRegion = _regions[currentPosition];

                    if (currentRegion.Contains(position))
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
    public static bool TryGet(out Region region, Vector2 position)
    {
        Vector2 anchor = Utility.WorldPositionToRegionPosition(position);
        Vector2 currentPosition = anchor;

        for (int x = 0; x < Region.SIZE; x++)
        {
            for (int y = 0; y < Region.SIZE; y++)
            {
                if (_regions.ContainsKey(currentPosition))
                {
                    Region currentRegion = _regions[currentPosition];

                    if (currentRegion.Contains(position))
                    {
                        region = currentRegion;
                        return true;
                    }
                }
            }
        }

        region = null;
        return false;
    }
}
