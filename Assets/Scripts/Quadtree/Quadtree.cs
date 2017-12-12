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

    private readonly QuadtreeNode<T> root;

    public void Insert(Rect rect, T obj)
    {
        root.Insert(new NodeObject<T>(rect, obj));
    }
    public void Draw(Color color)
    {
        root.Draw(color);
    }
}
