using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region {

    public Region(Vector2 position)
    {
        _position = position;
    }
    public Region(Vector2 position, Vector2 size)
    {
        _position = position;
        _size = size;

        if (size == Vector2.zero)
            throw new System.ArgumentException("Region size must be greater than 0. Parsed size was " + size);
    }

    public const int SIZE = 8;

    public Vector2 Position { get { return _position; } }
    public Vector2 Size { get { return _size; } }

    private readonly Vector2 _position;

    private Vector2 _size;

    public void Allocate(Vector2 position)
    {
        Vector2 delta = position - this._position;

        if(delta.x >= SIZE || delta.y >= SIZE)
            throw new System.ArgumentOutOfRangeException();

        _size = new Vector2()
        {
            x = Mathf.Max(_size.x, delta.x),
            y = Mathf.Max(_size.y, delta.y),
        };
    }
    public bool Contains(Vector2 worldPosition)
    {
        Vector2 delta = worldPosition - Position;

        return delta.x >= 0 && delta.y >= 0 && delta.x < SIZE && delta.y < SIZE;
    }
    public override bool Equals(object obj)
    {
        if(obj is Region)
        {
            Region other = obj as Region;

            throw new System.NotImplementedException("Add comparison of contents");
            return other._position == this._position && other._size == this._size;
        }

        return false;
    }
    public override int GetHashCode()
    {
        return _position.GetHashCode();
    }
    public override string ToString()
    {
        return string.Format("{0} - {1}", _position, _size);
    }
}
