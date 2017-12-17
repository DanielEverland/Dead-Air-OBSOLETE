using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree<T> {

    private Quadtree() { }

    public Quadtree(int size)
    {
        if (!size.IsPowerOfTwo())
        {
            throw new System.ArgumentException("Size must be power of two!");
        }

        root = new QuadtreeNode<T>(new Rect(0, 0, size, size));
    }

    public int Count { get { return _count; } }

    private readonly QuadtreeNode<T> root;

    private int _count;

    public void Insert(Rect rect, T obj)
    {
        root.Insert(new NodeObject<T>(rect, obj));

        _count++;
    }
    public List<T> Query(Rect rect)
    {
        return root.Query(rect);
    }
    public void Draw()
    {
        root.Draw();
    }
}