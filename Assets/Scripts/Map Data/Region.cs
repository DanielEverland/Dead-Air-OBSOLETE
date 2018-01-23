using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region {

    public Region(Vector2 position)
    {
        _position = position;
    }

    public const int MAX_SIZE = 8;

    public Vector2 Position { get { return _position; } }
    public int CellCount { get { return _ownedPositions.Count; } }

    private readonly Vector2 _position;

    private List<Vector2> _ownedPositions = new List<Vector2>();

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
