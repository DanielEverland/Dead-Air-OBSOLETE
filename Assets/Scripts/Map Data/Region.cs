using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region {

    public Region()
    {
        SetDirty();
    }
    public Region(Vector2 position)
    {
        _position = position;

        SetDirty();
    }

    public const int MAX_SIZE = 8;

    private static Queue<Region> _dirtyRegions = new Queue<Region>();

    public Rect Bounds { get { return _bounds; } }
    public Vector2 Position { get { return _position; } }
    public int CellCount { get { return _ownedPositions.Count; } }
    public IEnumerable<Vector2> OwnedPositions { get { return _ownedPositions; } }

    private readonly Vector2 _position;

    private List<Vector2> _ownedPositions = new List<Vector2>();
    private Rect _bounds;

    public void Allocate(Vector2 position)
    {
        Vector2 delta = position - this.Position;

        if (delta.x < 0 || delta.y < 0 || delta.x >= MAX_SIZE || delta.y >= MAX_SIZE)
            throw new System.IndexOutOfRangeException();

        _ownedPositions.Add(position);
    }
    public bool Contains(Vector2 worldPosition)
    {
        Vector2 delta = worldPosition - Position;

        return delta.x >= 0 && delta.y >= 0 && delta.x < MAX_SIZE && delta.y < MAX_SIZE;
    }
    public void SetDirty()
    {
        if (!_dirtyRegions.Contains(this))
            _dirtyRegions.Enqueue(this);
    }
    private void Clean()
    {
        RecalculateBounds();
    }
    private void RecalculateBounds()
    {
        float xMax = _ownedPositions.Max(x => x.x) + 1;
        float yMax = _ownedPositions.Max(x => x.y) + 1;

        Vector2 size = new Vector2(xMax - _position.x, yMax - _position.y);

        _bounds = new Rect(_position, size);
    }
    public static void CleanDirtyRegions()
    {
        while (_dirtyRegions.Count > 0)
        {
            _dirtyRegions.Dequeue().Clean();
        }
    }
    public override bool Equals(object obj)
    {
        if(obj is Region)
        {
            Region other = obj as Region;
            
            return other._position == this._position && other._ownedPositions == this._ownedPositions;
        }

        return false;
    }
    public override int GetHashCode()
    {
        return _position.GetHashCode();
    }
    public override string ToString()
    {
        return string.Format("{0} - ({1})", _position, _ownedPositions.Count);
    }
}
