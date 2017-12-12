using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NodeObject<T>
{
    public NodeObject(Rect rect, T obj)
    {
        _rect = rect;
        _obj = obj; 
    }

    public Rect Rect { get { return _rect; } }
    public T Object { get { return _obj; } }

    private readonly Rect _rect;
    private readonly T _obj;
}
