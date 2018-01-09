using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Player Player { get { return Player.Instance; } }

    private static List<Action> LoadFlow = new List<Action>()
    {
        TileType.LoadAllTileTypes,
        ChunkGenerator.Initialize,
        MapDataManager.Initialize,
        PlayerCamera.Center,
    };

    private void Start()
    {
        InitializeGame();
    }
    private void Update()
    {
        MapData.RefreshQuadtree();
        MapData.EntityQuadtree.Draw();
    }
    private static void InitializeGame()
    {
        for (int i = 0; i < LoadFlow.Count; i++)
        {
            LoadFlow[i].Invoke();
        }
    }
    public static Entity CreateEntity<T>() where T : Entity
    {
        return MapData.CreateEntity<T>();
    }
    public static IEnumerable<T> GetEntitiesOfType<T>() where T : Entity
    {
        return MapData.Entities.Where(x => x is T).Select(x => x as T);
    }
    public static List<T> GetEntitiesWithinRange<T>(Vector2 center, float radius) where T : Entity
    {
        Rect rect = new Rect(center - new Vector2(radius, radius), new Vector2(radius * 2, radius * 2));
        List<Entity> entities = MapData.EntityQuadtree.Query(rect);
        List<T> toReturn = new List<T>();

        for (int i = 0; i < entities.Count; i++)
        {
            Entity entity = entities[i];

            if(entity is T)
            {
                if(Vector2.Distance(entity.transform.position, center) <= radius)
                {
                    toReturn.Add(entity as T);
                }
            }
        }

        return toReturn;
    }
}
