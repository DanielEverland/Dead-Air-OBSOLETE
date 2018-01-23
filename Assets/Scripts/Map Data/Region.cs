using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region {

    public Region()
    {
        FlagForReinitialization();
    }
    public Region(Vector2 position)
    {
        _anchor = Utility.WorldPositionToRegionPosition(position);

        FlagForReinitialization();
    }

    public const int MAX_SIZE = 8;

    private static Queue<Region> _regionsToInitialize = new Queue<Region>();

    public Rect Bounds { get { return _bounds; } }
    public Vector2 Position { get { return Bounds.position; } }
    public int CellCount { get { return _ownedPositions.Count; } }
    public IEnumerable<Vector2> OwnedPositions { get { return _ownedPositions; } }

    private readonly Vector2 _anchor;

    private List<Vector2> _ownedPositions = new List<Vector2>();
    private Rect _bounds;

    public void Allocate(Vector2 position)
    {
        Vector2 delta = position - _anchor;

        if (delta.x < 0 || delta.y < 0 || delta.x >= MAX_SIZE || delta.y >= MAX_SIZE)
            throw new System.IndexOutOfRangeException(delta + " - " + position);

        _ownedPositions.Add(position);
    }
    public bool Contains(Vector2 worldPosition)
    {
        Vector2 delta = worldPosition - Position;

        return delta.x >= 0 && delta.y >= 0 && delta.x < MAX_SIZE && delta.y < MAX_SIZE;
    }
    public void FlagForReinitialization()
    {
        if (!_regionsToInitialize.Contains(this))
            _regionsToInitialize.Enqueue(this);
    }
    private void Initialize()
    {
        RecalculateBounds();
    }
    private void RecalculateBounds()
    {
        if (_ownedPositions.Count <= 0)
            return;

        float xMin = _ownedPositions.Min(x => x.x);
        float yMin = _ownedPositions.Min(x => x.y);
        float xMax = _ownedPositions.Max(x => x.x) + 1;
        float yMax = _ownedPositions.Max(x => x.y) + 1;
        
        _bounds = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
    public static void InitializeRegions()
    {
        while (_regionsToInitialize.Count > 0)
        {
            _regionsToInitialize.Dequeue().Initialize();
        }
    }
    public override bool Equals(object obj)
    {
        if(obj is Region)
        {
            Region other = obj as Region;
            
            return other._ownedPositions == this._ownedPositions;
        }

        return false;
    }
    public override int GetHashCode()
    {
        return _ownedPositions.GetHashCode();
    }
    public override string ToString()
    {
        return string.Format("{0} - ({1})", Bounds, _ownedPositions.Count);
    }
}
