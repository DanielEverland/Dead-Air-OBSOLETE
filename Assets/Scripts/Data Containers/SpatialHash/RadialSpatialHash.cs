using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialSpatialHash<T> : SpatialHash<T, Circle> {
    
    public RadialSpatialHash(float cellSize) : base(cellSize) { }
    
    public void Insert(T obj, Vector2 center, float radius)
    {
        Circle circle = new Circle(center, radius);

        foreach (Vector2 bucketPos in GetAllPossibleBuckets(center, radius))
        {
            InsertObject(obj, circle, bucketPos);
        }
    }
    public bool Contains(Vector2 center, float radius)
    {
        foreach (Vector2 bucketPos in GetAllPossibleBuckets(center, radius))
        {
            if (ContainsObjects(bucketPos))
                return true;
        }

        return false;
    }
    public List<T> Get(Vector2 center, float radius)
    {
        List<T> objects = new List<T>();

        foreach (Vector2 bucketPos in GetAllPossibleBuckets(center, radius))
        {
            objects.AddRange(FilterDataEntries(center, radius, GetObjects(bucketPos)));
        }

        return objects;
    }
    private List<T> FilterDataEntries(Vector2 center, float radius, List<DataEntry<T, Circle>> entries)
    {
        List<T> objsToReturn = new List<T>();
        Circle circle = new Circle(center, radius);

        for (int i = 0; i < entries.Count; i++)
        {
            if (circle.Collides(entries[i].Key))
                objsToReturn.Add(entries[i].Object);
        }

        return objsToReturn;
    }
    public List<Vector2> GetAllPossibleBuckets(Vector2 center, float radius)
    {
        List<Vector2> allPositions = new List<Vector2>();

        int xMin = Mathf.FloorToInt((center.x - radius) / CellSize);
        int xMax = Mathf.FloorToInt((center.x + radius) / CellSize);

        int yMin = Mathf.FloorToInt((center.y - radius) / CellSize);
        int yMax = Mathf.FloorToInt((center.y + radius) / CellSize);

        for (int x = 0; x <= xMax - xMin; x++)
        {
            for (int y = 0; y <= yMax - yMin; y++)
            {
                allPositions.Add(new Vector2(xMin + x, yMin + y));
            }
        }

        return allPositions;
    }
#if UNITY_EDITOR
    public override void Draw()
    {
        base.Draw();

        if (DebugData.SpatialHashDrawObjects)
        {
            foreach (DataEntry<T, Circle> entry in Entries)
            {
                EG_Debug.DrawCircle(entry.Key.center, entry.Key.radius);
            }
        }        
    }
#endif
}
