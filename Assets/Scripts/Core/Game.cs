using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Player Player { get { return Player.Instance; } }
    public static bool HasLoaded { get { return LoadManager.HasLoaded; } }
    
    private void Update()
    {
        if(!HasLoaded)
        {
            LoadManager.Update();
            return;
        }
        
        MapData.Update();
        VisionManager.Update();
        DebugMenu.GlobalUpdate();
        RegionManager.Update();

        #region DEBUG Remove me
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            MapData.SetWallTile(Camera.main.ScreenToWorldPoint(Input.mousePosition), WallType.AllTypes[0].ID);
        }
        #endregion
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
