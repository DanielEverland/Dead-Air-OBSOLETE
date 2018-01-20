using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spatial hash for storing objects in buckets
/// </summary>
/// <typeparam name="T1">Object type</typeparam>
/// <typeparam name="T2">Key type</typeparam>
public class SpatialHash<T1, T2> {

    private SpatialHash() { }
    public SpatialHash(float cellSize)
    {
        if (!MapData.MAPSIZE.IsDivisible(cellSize))
        {
            float cellCount = MapData.MAPSIZE / cellSize;
            float flooredCellCount = Mathf.FloorToInt(cellCount);
            float newCellSize = Mathf.FloorToInt(cellCount);

            Debug.Log("Adjusting cellsize since mapsize " + MapData.MAPSIZE + " is not divisable by cellsize " + cellSize + ". New cellsize is " + newCellSize);

            cellSize = newCellSize;
        }

        _cellSize = cellSize;

        InitializeBuckets();
    }

    public List<T1> this[Vector2 bucketPosition]
    {
        get
        {
            return (List<T1>)_buckets[bucketPosition].Select(x => x.Object);
        }
    }

    public IEnumerable<DataEntry<T1, T2>> Entries
    {
        get
        {
            List<DataEntry<T1, T2>> entriesToReturn = new List<DataEntry<T1, T2>>();

            foreach (KeyValuePair<Vector2, List<DataEntry<T1, T2>>> keyValuePair in _buckets)
            {
                if (keyValuePair.Value.Count > 0)
                    entriesToReturn.AddRange(keyValuePair.Value);
            }

            return entriesToReturn;
        }
    }

    public IEnumerable<T1> Objects { get { return Entries.Select(x => x.Object); } }
    public float CellSize { get { return _cellSize; } }

    private readonly float _cellSize;
    private Dictionary<Vector2, List<DataEntry<T1, T2>>> _buckets;
    private Dictionary<Vector2, int> _bucketUsedFrameCount;

    private Color IdleBucketColor { get { return Color.red; } }
    private Color UsedBucketColor { get { return Color.cyan; } }

    private const int UsedFrameMargin = 3;

    protected void InitializeBuckets()
    {
        _buckets = new Dictionary<Vector2, List<DataEntry<T1, T2>>>();
        _bucketUsedFrameCount = new Dictionary<Vector2, int>();

        int bucketsSquared = (int)((float)MapData.MAPSIZE / CellSize);

        for (int x = 0; x < bucketsSquared; x++)
        {
            for (int y = 0; y < bucketsSquared; y++)
            {
                _buckets.Add(new Vector2(x, y), new List<DataEntry<T1, T2>>());
            }
        }
    }
    public void Clear()
    {
        InitializeBuckets();
    }
    protected bool ContainsObjects(Vector2 worldPosition)
    {
        Vector2 bucketPosition = WorldToBucketPosition(worldPosition);

        if (!_buckets.ContainsKey(bucketPosition))
            return false;

        Tick(bucketPosition);

        return _buckets[bucketPosition].Count > 0;
    }
    protected void InsertObject(T1 obj, T2 key, Vector2 worldPosition)
    {
        Vector2 bucketPosition = WorldToBucketPosition(worldPosition);

        if (!_buckets.ContainsKey(bucketPosition))
            return;

        Tick(bucketPosition);

        _buckets[bucketPosition].Add(new DataEntry<T1, T2>(obj, key));
    }
    protected List<DataEntry<T1, T2>> GetObjects(Vector2 worldPosition)
    {
        Vector2 bucketPosition = WorldToBucketPosition(worldPosition);

        if (!_buckets.ContainsKey(bucketPosition))
            return new List<DataEntry<T1, T2>>();

        Tick(bucketPosition);

        return _buckets[bucketPosition];
    }
    protected Vector2 WorldToBucketPosition(Vector2 worldPosition)
    {
        return new Vector2()
        {
            x = Mathf.FloorToInt(worldPosition.x / CellSize),
            y = Mathf.FloorToInt(worldPosition.y / CellSize),
        };
    }
    public virtual void Tick(Vector2 bucketPosition)
    {
        if (!_bucketUsedFrameCount.ContainsKey(bucketPosition))
        {
            _bucketUsedFrameCount.Add(bucketPosition, Time.frameCount);
        }
        else
        {
            _bucketUsedFrameCount[bucketPosition] = Time.frameCount;
        }
    }
    protected bool WasUsedRecently(Vector2 position)
    {
        if (!_bucketUsedFrameCount.ContainsKey(position))
            return false;

        return Time.frameCount - _bucketUsedFrameCount[position] <= UsedFrameMargin;
    } 
    public virtual void Draw()
    {
        if (!DebugData.SpatialHashDrawAllBuckets && !DebugData.SpatialHashDrawTicks)
            return;

        foreach (Vector2 bucketPosition in _buckets.Keys)
        {
            Rect rect = new Rect(bucketPosition * CellSize, Vector2.one * CellSize);

            if(DebugData.SpatialHashDrawAllBuckets)
                EG_Debug.DrawRect(rect, IdleBucketColor);

            if(DebugData.SpatialHashDrawTicks)
                EG_Debug.DrawRect(rect.Shrink(0.1f), UsedBucketColor);
        }
    }
}
