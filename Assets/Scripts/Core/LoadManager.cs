using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoadManager {

    public static bool HasLoaded { get { return _hasLoaded; } }

    private static readonly List<LoadEntry> _loadFlow = new List<LoadEntry>()
    {
        //Data Initialization
        new LoadEntry(){ function = EG_Debug.Initialize, loadText = "Initializing Debug Functions" },
        new LoadEntry(){ function = TileType.LoadAllTileTypes, loadText = "Loading Tiletypes" },

        //Map Generation
        new LoadEntry(){ function = ChunkGenerator.Initialize, loadText = "Initializing Chunk Generator" },
        new LoadEntry(){ function = MapDataManager.Initialize, loadText = "Loading Map" },
        new LoadEntry(){ function = RegionManager.Initialize, loadText = "Creating Regions" },

        //Scene Initialization
        new LoadEntry(){ function = PlayerCamera.Initialize, loadText = "Camera Setup" },
        new LoadEntry(){ function = DebugManager.Initialize, loadText = "Loading Developer Options" },
    };

    private static LoadingScreen _loadingScreen;
    private static bool _hasInitialized = false;
    private static bool _hasLoaded = false;
    private static int _loadStep = -1;

    public static void Update()
    {
        if (!_hasInitialized)
            Initialize();

        Poll();
    }
    private static void Poll()
    {
        if (HasLoaded)
            return;

        LoadEntry current = _loadFlow[_loadStep];

        if(current.hasLoaded)
        {
            Next();
        }
        else
        {
            Execute();
        }
    }
    private static void Next()
    {
        _loadStep++;

        if(_loadStep == _loadFlow.Count)
        {
            Finish();
            return;
        }

        LoadEntry current = _loadFlow[_loadStep];

        _loadingScreen.SetProgress((float)_loadStep / (float)_loadFlow.Count);
        _loadingScreen.SetText(current.loadText);
    }
    private static void Execute()
    {
        LoadEntry current = _loadFlow[_loadStep];

        current.hasLoaded = current.function.Invoke();
    }
    private static void Initialize()
    {
        _loadingScreen = PrefabPool.GetObject<LoadingScreen>("LoadingScreen");

        Next();

        _hasInitialized = true;
    }
    private static void Finish()
    {
        _hasLoaded = true;
        GameObject.Destroy(_loadingScreen.gameObject);
    }
    private class LoadEntry
    {
        public Func<bool> function;
        public string loadText;
        public bool hasLoaded;
    }
}
