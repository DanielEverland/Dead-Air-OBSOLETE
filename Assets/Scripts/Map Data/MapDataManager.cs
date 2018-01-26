using System.Linq;
using System;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataManager : MonoBehaviour {
    
    public static MapData CurrentlyLoadedMap { get { return currentlyLoadedMap; } }

    private static MapData currentlyLoadedMap;
       
    public static bool Initialize()
    {
        currentlyLoadedMap = new MapData();

        LoadMap();

        return true;
    }
    public static void LoadMap()
    {
        foreach (Chunk chunk in MapData.ChunkObjects)
        {
            ChunkGenerator.RenderChunk(chunk);
        }
    }
}
