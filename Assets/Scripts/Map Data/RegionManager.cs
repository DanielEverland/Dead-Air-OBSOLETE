using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RegionManager {

    private static Dictionary<Vector2, Region> _regions;
    
    public static void Initialize()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        ExecuteFullRegeneration();

        stopWatch.Stop();
        UnityEngine.Debug.Log(stopWatch.Elapsed);
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
    private static Region Create(Vector2 position)
    {
        Region region = new Region(position);
        Queue<Vector2> queue = new Queue<Vector2>();
        HashSet<Vector2> checkedPositions = new HashSet<Vector2>();

        Vector2 anchor = Utility.WorldPositionToRegionPosition(position);
        Vector2 anchorDelta = position - anchor;
        Vector2 maxSize = new Vector2(Region.MAX_SIZE - anchorDelta.x, Region.MAX_SIZE - anchorDelta.y);
                
        queue.Enqueue(position);
        while (queue.Count > 0)
        {
            Vector2 current = queue.Dequeue();
            
            region.Allocate(current);
            _regions.Add(current, region);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 loopPos = current + new Vector2(x, y);
                    Vector2 loopDelta = loopPos - position;

                    if (loopDelta.x >= 0 && loopDelta.y >= 0 && loopDelta.x < maxSize.x && loopDelta.y < maxSize.y
                        && !checkedPositions.Contains(loopPos) && !queue.Contains(loopPos) && !_regions.ContainsKey(loopPos))
                    {
                        queue.Enqueue(loopPos);
                        checkedPositions.Add(loopPos);
                    }
                }
            }
        }
        
        return region;
    }
    private static bool Contains(Vector2 position)
    {
        Vector2 floored = new Vector2()
        {
            x = Mathf.FloorToInt(position.x),
            y = Mathf.FloorToInt(position.y),
        };

        return _regions.ContainsKey(floored);
    }
    public static Region GetRegion(Vector2 position)
    {
        Vector2 floored = new Vector2()
        {
            x = Mathf.FloorToInt(position.x),
            y = Mathf.FloorToInt(position.y),
        };

        if(_regions.ContainsKey(floored))
        {
            return _regions[floored];
        }
        else
        {
            return null;
        }
    }
    public static void Draw()
    {
        if (DebugData.RegionsDrawAll)
        {
            Vector2 mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Contains(mousePosInWorld))
            {
                Region region = GetRegion(mousePosInWorld);
                EG_Debug.DrawRect(new Rect(region.Position, Vector2.one * Region.MAX_SIZE), Color.white);
                UnityEngine.Debug.DrawLine(mousePosInWorld, region.Position);
            }
        }
    }
}
