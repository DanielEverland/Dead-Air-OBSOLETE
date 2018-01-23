using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Player Player { get { return Player.Instance; } }
    
    private static List<Action> LoadFlow = new List<Action>()
    {
        //Data Initialization
        EG_Debug.Initialize,
        TileType.LoadAllTileTypes,

        //Map Generation
        ChunkGenerator.Initialize,
        MapDataManager.Initialize,
        RegionManager.Initialize,

        //Scene Initialization
        PlayerCamera.Center,
        DebugManager.Initialize,
    };
    
    private void Start()
    {
        InitializeGame();

        EG_Debug.DrawRect(new Rect(25, 25, 89, 989), Color.red, 10);
    }
    private void Update()
    {
        MapData.Update();
        VisionManager.Update();
        DebugMenu.GlobalUpdate();

        #region DEBUG Remove me
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            MapData.SetWallTile(Camera.main.ScreenToWorldPoint(Input.mousePosition), WallType.AllTypes[0].ID);
        }
        #endregion
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
