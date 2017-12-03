using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Player Player { get { return Player.Instance; } }

    private static List<Action> LoadFlow = new List<Action>()
    {
        TileType.LoadAllTileTypes,
        ChunkGenerator.Initialize,
    };

    private void Start()
    {
        InitializeGame();
    }
    private static void InitializeGame()
    {
        for (int i = 0; i < LoadFlow.Count; i++)
        {
            LoadFlow[i].Invoke();
        }
    }
}
