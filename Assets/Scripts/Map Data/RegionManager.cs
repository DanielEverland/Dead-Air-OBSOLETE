using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RegionManager {

    public static IEnumerable<Region> Regions { get { return _regions; } }
    
    private static List<Vector2> _dirtyCells;
    private static Dictionary<Vector2, Region> _regionPositions;
    private static HashSet<Region> _regions = new HashSet<Region>();

    private static readonly Color DEBUG_REGION_EDGE_COLOR = new Color(0, 1, 1, 0.3f);
    private static readonly Color DEBUG_SELECTED_REGION_COLOR = new Color(1, 0, 1, 0.6f);
    private static readonly Color DEBUG_NEIGHBOR_REGION_COLOR = new Color(1, 1, 1, 0.6f);
    private static readonly Color DEBUG_REGION_ENTITY_COLOR = new Color(1, 1, 0, 1);

    public static bool Initialize()
    {
        ExecuteFullRegeneration();

        return true;
    }
    public static void Update()
    {
        Region.CleanDirtyRegions();
        CleanDirtyCells();
    }
    public static bool ContainsCell(Vector2 cellPosition)
    {
        return _regionPositions.ContainsKey(cellPosition.ToCellPosition());
    }
    public static bool ContainsRegion(Region reg)
    {
        return _regions.Contains(reg);
    }
    public static void CleanDirtyCells()
    {
        while (_dirtyCells.Count > 0)
        {
           Create(_dirtyCells[0], true);
        }
    }
    public static void SetDirty(Vector2 position)
    {
        position = position.ToCellPosition();

        if (_dirtyCells.Contains(position) || !_regionPositions.ContainsKey(position))
            return;
        
        Region region = _regionPositions[position];
        RegionConnectionManager.Remove(region);
        _regions.Remove(region);

        foreach (Vector2 ownedPos in region.OwnedPositions)
        {
            _regionPositions.Remove(ownedPos);
            _dirtyCells.Add(ownedPos);
        }
    }
    private static void ExecuteFullRegeneration()
    {
        foreach (Region region in _regions)
        {
            RegionConnectionManager.Remove(region);
        }

        _dirtyCells = new List<Vector2>();
        _regionPositions = new Dictionary<Vector2, Region>();
        _regions = new HashSet<Region>();

        for (int x = 0; x < MapData.MAPSIZE; x++)
        {
            for (int y = 0; y < MapData.MAPSIZE; y++)
            {
                Vector2 pos = new Vector2(x, y);

                if (!Contains(pos) && IsValid(pos))
                    Create(pos);
            }
        }
    }
    private static Region Create(Vector2 position, bool informNeighbors = false)
    {
        Region region = new Region(position);
        Queue<Vector2> queue = new Queue<Vector2>();
        HashSet<Vector2> checkedPositions = new HashSet<Vector2>();

        Vector2 anchor = Utility.WorldPositionToRegionPosition(position);

        List<Vector2> DEBUG_DRAWS = new List<Vector2>();

        queue.Enqueue(position);
        while (queue.Count > 0)
        {
            Vector2 current = queue.Dequeue();
            
            if (_dirtyCells.Contains(current))
                _dirtyCells.Remove(current);

            if (!MapData.IsValidPosition(current) || !MapData.IsPassable(current))
                continue;

            if(DebugData.RegionsFloodFillDebug)
                DEBUG_DRAWS.Add(current);

            region.Allocate(current);
            _regionPositions.Add(current, region);
                        
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 loopPos = current + new Vector2(x, y);
                    Vector2 loopDelta = loopPos - anchor;

                    if (loopDelta.x >= 0 && loopDelta.y >= 0 && loopDelta.x < Region.MAX_SIZE && loopDelta.y < Region.MAX_SIZE
                        && !checkedPositions.Contains(loopPos) && !queue.Contains(loopPos) && !_regionPositions.ContainsKey(loopPos)
                        && IsValid(loopPos))
                    {
                        queue.Enqueue(loopPos);
                        checkedPositions.Add(loopPos);
                    }
                }
            }
        }

        for (float i = 0; i < DEBUG_DRAWS.Count; i++)
        {
            float percentage = i / (float)DEBUG_DRAWS.Count;

            Color color = new Color(0, 0, percentage);

            EG_Debug.DrawSquare(new Rect(DEBUG_DRAWS[(int)i], Vector2.one), color, 2);
        }

        region.Initialize();
        _regions.Add(region);

        if(informNeighbors)
            region.SetDirty(true);

        return region;
    }
    private static bool IsValid(Vector2 position)
    {
        Vector2 floored = new Vector2()
        {
            x = Mathf.FloorToInt(position.x),
            y = Mathf.FloorToInt(position.y),
        };

        return MapData.IsPassable(position) && MapData.IsValidPosition(position);
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

                EG_Debug.DrawText("----------SELECTED REGION----------\n" + region);

                foreach (int connection in region.Connections)
                {
                    EG_Debug.DrawText(connection + ": " + RegionConnectionManager.GetRegions(connection).Count());
                }

                foreach (Vector2 pos in region.OwnedPositions)
                {
                    EG_Debug.DrawSquare(new Rect(pos, Vector2.one), DEBUG_SELECTED_REGION_COLOR);
                }

                if(DebugData.RegionsDrawEntities)
                {
                    foreach (Entity entity in region.Entities)
                    {
                        EG_Debug.DrawRect(entity.Rect, DEBUG_REGION_ENTITY_COLOR);
                    }
                }
                
                if(DebugData.RegionsDrawNeighbors)
                {
                    foreach (Region neighbor in region.Neighbors)
                    {
                        foreach (Vector2 pos in neighbor.OwnedPositions)
                        {
                            EG_Debug.DrawSquare(new Rect(pos, Vector2.one), DEBUG_NEIGHBOR_REGION_COLOR);
                        }
                    }
                }

                if(DebugData.RegionsDrawEdges)
                {
                    foreach (Region.Connection connection in region.Connections)
                    {
                        Vector2 pos = connection.Position;
                        Vector2 size = new Vector2((connection.Normal.x == 0) ? 1 : connection.Normal.x * connection.Length, (connection.Normal.y == 0) ? 1 : connection.Normal.y * connection.Length);

                        Rect rect = new Rect(pos, size);

                        EG_Debug.DrawSquare(rect, DEBUG_REGION_EDGE_COLOR);
                        EG_Debug.DrawRect(rect, Color.black);
                    }
                }
            }
        }
    }
    
}
