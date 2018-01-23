using System.Diagnostics;
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

        for (int x = 0; x < MapData.MAPSIZE; x++)
        {
            for (int y = 0; y < MapData.MAPSIZE; y++)
            {
                Vector2 pos = new Vector2(x, y);

                if (!Contains(pos))
                    Create(pos);
            }
        }
    }
    private static Region Create(Vector2 position, bool addToList = true)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        HashSet<Vector2> checkedPositions = new HashSet<Vector2>();

        Vector2 anchor = Utility.WorldPositionToRegionPosition(position);
        Vector2 anchorDelta = position - anchor;
        Vector2 maxSize = new Vector2(Region.SIZE - anchorDelta.x + 1, Region.SIZE - anchorDelta.y + 1);

        Vector2 size = new Vector2();
        
        queue.Enqueue(position);
        while (queue.Count > 0)
        {
            Vector2 current = queue.Dequeue();
            checkedPositions.Add(current);
            
            Vector2 delta = current - position;

            size.x = Mathf.Max(size.x, delta.x);
            size.y = Mathf.Max(size.y, delta.y);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 loopPos = current + new Vector2(x, y);
                    Vector2 loopDelta = loopPos - position;

                    if (loopDelta.x >= 0 && loopDelta.y >= 0 && loopDelta.x < maxSize.x && loopDelta.y < maxSize.y
                        && !checkedPositions.Contains(loopPos) && !queue.Contains(loopPos))
                    {
                        queue.Enqueue(loopPos);
                    }
                }
            }
        }

        Region region = new Region(position, size);

        if (addToList)
            Add(region);

        return region;
    }
    private static bool Contains(Vector2 position)
    {
        Vector2 anchor = Utility.WorldPositionToRegionPosition(position);

        for (int x = 0; x < Region.SIZE; x++)
        {
            for (int y = 0; y < Region.SIZE; y++)
            {
                Vector2 currentPosition = anchor + new Vector2(x, y);

                if(_regions.ContainsKey(currentPosition))
                {
                    if (_regions[currentPosition].Contains(position))
                        return true;
                }
            }
        }

        return false;
    }
    private static void Add(Region region)
    {
        _regions.Add(region.Position, region);
    }
    public static void Draw()
    {
        if (DebugData.RegionsDrawAll)
        {
            foreach (Region region in _regions.Values)
            {
                EG_Debug.DrawRect(new Rect(region.Position, region.Size), Color.white);
            }
        }
    }
}
