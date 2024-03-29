﻿public static class DebugData {

	[EG_Debug.Toggle("General", "Godmode", false)]
    public static bool GodMode { get; set; }



    #region Quadtree
    [EG_Debug.Toggle("Data Structures", "Draw All Rects", false, Header = "Quadtree")]
    public static bool QuadtreeDrawAllRects { get; set; }

    [EG_Debug.Toggle("Data Structures", "Draw Ticks", false, Header = "Quadtree")]
    public static bool QuadtreeDrawTicks { get; set; }

    [EG_Debug.Toggle("Data Structures", "Draw Objects", false, Header = "Quadtree")]
    public static bool QuadtreeDrawObjects { get; set; }
    #endregion

    #region Spatial Hash
    [EG_Debug.Toggle("Data Structures", "Draw All Buckets", false, Header = "SpatialHash")]
    public static bool SpatialHashDrawAllBuckets { get; set; }

    [EG_Debug.Toggle("Data Structures", "Draw Ticks", false, Header = "SpatialHash")]
    public static bool SpatialHashDrawTicks { get; set; }

    [EG_Debug.Toggle("Data Structures", "Draw Objects", false, Header = "SpatialHash")]
    public static bool SpatialHashDrawObjects { get; set; }
    #endregion

    #region Regions
    [EG_Debug.Toggle("Data Structures", "Draw All Region Bounds", true, Header = "Regions")]
    public static bool RegionsDrawAllBounds { get; set; }

    [EG_Debug.Toggle("Data Structures", "Draw Selected Region", true, Header = "Regions")]
    public static bool RegionsDrawSelected { get; set; }

    [EG_Debug.Toggle("Data Structures", "Draw Selected Regions Edges", true, Header = "Regions")]
    public static bool RegionsDrawEdges { get; set; }

    [EG_Debug.Toggle("Data Structures", "Draw Region Flood Fill", false, Header = "Regions")]
    public static bool RegionsFloodFillDebug { get; set; }

    [EG_Debug.Toggle("Data Structures", "Draw Neighbors", true, Header = "Regions")]
    public static bool RegionsDrawNeighbors { get; set; }

    [EG_Debug.Toggle("Data Structures", "Draw Entities", false, Header = "Regions")]
    public static bool RegionsDrawEntities { get; set; }
    #endregion

    #region Pathfinding
    [EG_Debug.Toggle("Pathfinding", "Draw Regional Paths", true)]
    public static bool PathfindingDrawRegionalPaths { get; set; }

    [EG_Debug.Toggle("Pathfinding", "Draw Flood Fill", true)]
    public static bool PathfindingDrawFloodFills { get; set; }

    [EG_Debug.Toggle("Pathfinding", "Draw Vector Field", true)]
    public static bool PathfindingDrawVectorField { get; set; }

    [EG_Debug.Toggle("Pathfinding", "Draw Connections", true)]
    public static bool PathfindingDrawConnections { get; set; }
    #endregion
}
