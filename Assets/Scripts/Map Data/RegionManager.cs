using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RegionManager {

    private static Dictionary<Vector2, Region> _regionPositions;
    private static List<Region> _regions;

    private static readonly Color DEBUG_SELECTED_REGION_COLOR = new Color(1, 0, 1, 0.1f);
    private static readonly Color DEBUG_NEIGHBOR_REGION_COLOR = new Color(1, 1, 1, 0.5f);

    public static void Initialize()
    {
        ExecuteFullRegeneration();
    }
    public static void Update()
    {
        Region.CleanDirtyRegions();
    }
    private static void ExecuteFullRegeneration()
    {
        _regionPositions = new Dictionary<Vector2, Region>();
        _regions = new List<Region>();

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
            _regionPositions.Add(current, region);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 loopPos = current + new Vector2(x, y);
                    Vector2 loopDelta = loopPos - position;

                    if (loopDelta.x >= 0 && loopDelta.y >= 0 && loopDelta.x < maxSize.x && loopDelta.y < maxSize.y
                        && !checkedPositions.Contains(loopPos) && !queue.Contains(loopPos) && !_regionPositions.ContainsKey(loopPos))
                    {
                        queue.Enqueue(loopPos);
                        checkedPositions.Add(loopPos);
                    }
                }
            }
        }

        _regions.Add(region);

        return region;
    }
    private static bool Contains(Vector2 position)
    {
        Vector2 floored = new Vector2()
        {
            x = Mathf.FloorToInt(position.x),
            y = Mathf.FloorToInt(position.y),
        };

        return _regionPositions.ContainsKey(floored);
    }
    public static Region GetRegion(Vector2 position)
    {
        Vector2 floored = new Vector2()
        {
            x = Mathf.FloorToInt(position.x),
            y = Mathf.FloorToInt(position.y),
        };

        if(_regionPositions.ContainsKey(floored))
        {
            return _regionPositions[floored];
        }
        else
        {
            return null;
        }
    }
    public static void Draw()
    {
        if (DebugData.RegionsDrawAllBounds)
        {
            foreach (Region region in _regions)
            {
                EG_Debug.DrawRect(region.Bounds, Color.white);
            }
        }

        if (DebugData.RegionsDrawSelected)
        {
            Vector2 mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Contains(mousePosInWorld))
            {
                Region region = GetRegion(mousePosInWorld);

                foreach (Vector2 pos in region.OwnedPositions)
                {
                    EG_Debug.DrawSquare(new Rect(pos, Vector2.one), DEBUG_SELECTED_REGION_COLOR);
                }
            }
        }
    }
}
