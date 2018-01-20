using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Player Player { get { return Player.Instance; } }

    [EG_Debug.Toggle("General", "Test Boolean Value", false)]
    private static bool TestProperty
    {
        get
        {
            return _testProperty;
        }
        set
        {
            _testProperty = value;

            Debug.Log(value);
        }
    }
    private static bool _testProperty;

    [EG_Debug.Toggle("General", "Test Boolean Value", false)]
    private static bool TestPropertya
    {
        get
        {
            return _testProperty;
        }
        set
        {
            _testProperty = value;

            Debug.Log(value);
        }
    }

    [EG_Debug.Toggle("General", "Test Boolean Value", false)]
    private static bool TestPropertyb
    {
        get
        {
            return _testProperty;
        }
        set
        {
            _testProperty = value;

            Debug.Log(value);
        }
    }
    [EG_Debug.Toggle("General", "Test Boolean Value", false)]
    private static bool TestPropertyba
    {
        get
        {
            return _testProperty;
        }
        set
        {
            _testProperty = value;

            Debug.Log(value);
        }
    }
    [EG_Debug.Toggle("General", "Test Boolean Value", false)]
    private static bool TestPropertybgf
    {
        get
        {
            return _testProperty;
        }
        set
        {
            _testProperty = value;

            Debug.Log(value);
        }
    }

    private static List<Action> LoadFlow = new List<Action>()
    {
        EG_Debug.Initialize,
        TileType.LoadAllTileTypes,
        ChunkGenerator.Initialize,
        MapDataManager.Initialize,
        PlayerCamera.Center,
        DebugManager.Initialize,
    };
    
    private void Start()
    {
        InitializeGame();
    }
    private void Update()
    {
        MapData.Update();
        VisionManager.Update();
        DebugMenu.GlobalUpdate();
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
    public static List<T> GetVisibleEntitiesWithinRange<T>(Vector2 center, float radius) where T : Entity
    {
        List<Entity> entities = MapData.GetAllVisibleEntities(center, radius);
        List<T> toReturn = new List<T>();

        for (int i = 0; i < entities.Count; i++)
        {
            Entity entity = entities[i];

            if (entity is T)
            {
                if (Vector2.Distance(entity.transform.position, center) <= radius)
                {
                    toReturn.Add(entity as T);
                }
            }
        }
        
        return toReturn;
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
