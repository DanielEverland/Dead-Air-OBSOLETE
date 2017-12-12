using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadtreeNode<T> {

    private QuadtreeNode() { }
    public QuadtreeNode(Rect rect, int maxObjectCount = DEFAULT_MAX_OBJECT_COUNT)
    {
        _rect = rect;
        _objects = new List<NodeObject<T>>(maxObjectCount);
        _maxObjects = maxObjectCount;
    }

    public const int DEFAULT_MAX_OBJECT_COUNT = 3;

    private readonly Rect _rect;
    private readonly List<NodeObject<T>> _objects;
    private readonly int _maxObjects;

    private bool HasChildNodes { get { return _childNodes != null; } }

    private ChildNodes<T> _childNodes;

    public void Insert(NodeObject<T> obj)
    {
        if(HasChildNodes)
        {
            if (_childNodes.Fits(obj))
            {
                _childNodes.Insert(obj);
            }
            else
            {
                Add(obj);
            }
        }
        else
        {
            Add(obj);
        }
    }
    private void PollSplit()
    {
        if (HasChildNodes)
            return;

        if (_objects.Count > _maxObjects)
            DoSplit();
    }
    private void DoSplit()
    {
        _childNodes = new ChildNodes<T>(this);
        List<NodeObject<T>> tempObjectList = new List<NodeObject<T>>(_objects);

        for (int i = 0; i < tempObjectList.Count; i++)
        {
            if (_childNodes.Fits(tempObjectList[i]))
            {
                _childNodes.Insert(tempObjectList[i]);
            }
        }
    }
    private void Add(NodeObject<T> obj)
    {
        _objects.Add(obj);

        PollSplit();
    }
    private void Remove(NodeObject<T> obj)
    {
        _objects.Remove(obj);
    }
    public bool Fits(NodeObject<T> obj)
    {
        return obj.Rect.Overlaps(_rect);
    }

    private class ChildNodes<T>
    {
        private ChildNodes() { }
        public ChildNodes(QuadtreeNode<T> parentNode)
        {
            _nodes = new QuadtreeNode<T>[4]
            {
                new QuadtreeNode<T>(parentNode._rect.GetFirstQuadrant(), parentNode._maxObjects),
                new QuadtreeNode<T>(parentNode._rect.GetSecondQuadrant(), parentNode._maxObjects),
                new QuadtreeNode<T>(parentNode._rect.GetThirdQuadrant(), parentNode._maxObjects),
                new QuadtreeNode<T>(parentNode._rect.GetFourthQuadrant(), parentNode._maxObjects),
            };
        }

        private readonly QuadtreeNode<T>[] _nodes;

        public bool Fits(NodeObject<T> obj)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].Fits(obj))
                {
                    return true;
                }
            }

            return false;
        }
        public void Insert(NodeObject<T> obj)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].Fits(obj))
                {
                    _nodes[i].Insert(obj);
                }
            }

            throw new System.ArgumentException("Rect doesn't fit in any child nodes. Call Fits before Insert");
        }
    }
}
