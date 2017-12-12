using System.Linq;
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
    private bool HasObjects { get { return _objects.Count > 0; } }

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
        return Utility.Encapsulates(obj.Rect, _rect);
    }
    public void Draw(Color color)
    {
        EG_Debug.DrawRect(_rect, color);

        if (HasChildNodes)
        {
            _childNodes.Do(x => x.Draw(color));
        }
    }
    public List<T> Query(Rect rect)
    {
        List<T> results = new List<T>();

        if (HasChildNodes)
        {
            if (Utility.Encapsulates(_rect, rect))
            {
                results.AddRange(AddObjectsRecursively());
            }
            else if (Utility.Intersects(rect, _rect))
            {
                _childNodes.Do(x => x.Query(rect));
            }
        }

        if (HasObjects)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if(Utility.Intersects(rect, _objects[i].Rect) || Utility.Encapsulates(_objects[i].Rect, rect))
                {
                    results.Add(_objects[i].Object);
                }
            }
        }

        return results;
    }
    private List<T> AddObjectsRecursively()
    {
        List<T> results = new List<T>();

        if(HasObjects)
        {
            results.AddRange(_objects.Select(x => x.Object));
        }

        if (HasChildNodes)
        {
            for (int i = 0; i < _childNodes.Count; i++)
            {
                results.AddRange(_childNodes[i].AddObjectsRecursively());
            }
        }

        return results;
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

        public QuadtreeNode<T> this[int index]
        {
            get
            {
                return _nodes[index];
            }
            set
            {
                _nodes[index] = value;
            }
        }

        public int Count { get { return _nodes.Length; } }

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
                    return;
                }
            }
            
            throw new System.ArgumentException("Rect " + obj.Rect + " doesn't fit in any child nodes. Call Fits before Insert");
        }
        public void Do(System.Action<QuadtreeNode<T>> action)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                action.Invoke(_nodes[i]);
            }
        }
    }
}
