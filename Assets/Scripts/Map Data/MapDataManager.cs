using System.Linq;
using System;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataManager : MonoBehaviour {

    public static bool HasLoadedMap { get { return CurrentlyLoadedMap != null; } }
    public static MapData CurrentlyLoadedMap { get { return currentlyLoadedMap; } }

    private static MapData currentlyLoadedMap;
       
    public static void Initialize()
    {
        if (HasLoadedMap)
            return;

        currentlyLoadedMap = new MapData();

        LoadMap();
    }
    public static void LoadMap()
    {
        foreach (Chunk chunk in MapData.ChunkObjects)
        {
            ChunkGenerator.RenderChunk(chunk);
        }
    }
}
