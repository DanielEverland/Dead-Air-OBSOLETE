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
    private int _lastActivityFrameNumber;

    private const byte LAST_ACTIVITY_DRAW_MARGIN = 2;
    private readonly Color DRAW_QUAD_COLOR = Color.red;
    private readonly Color DRAW_ACTIVITY_COLOR = Color.cyan;
    private readonly Color DRAW_OBJECT_COLOR = Color.white;

    public void Insert(NodeObject<T> obj)
    {
        PollActivity();

        if (HasChildNodes)
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

        PollActivity();

        if (_objects.Count > _maxObjects)
            DoSplit();
    }
    private void DoSplit()
    {
        _childNodes = new ChildNodes<T>(this);
        List<NodeObject<T>> tempObjectList = new List<NodeObject<T>>(_objects);
        _objects.Clear();

        for (int i = 0; i < tempObjectList.Count; i++)
        {
            if (_childNodes.Fits(tempObjectList[i]))
            {
                _childNodes.Insert(tempObjectList[i]);
            }
            else
            {
                _objects.Add(tempObjectList[i]);
            }
        }

        PollActivity();
    }
    private void Add(NodeObject<T> obj)
    {
        _objects.Add(obj);

        PollSplit();
        PollActivity();
    }
    private void Remove(NodeObject<T> obj)
    {
        _objects.Remove(obj);

        PollActivity();
    }
    private void PollActivity()
    {
        _lastActivityFrameNumber = Time.frameCount;
    }
    public bool Fits(NodeObject<T> obj)
    {
        return Utility.Encapsulates(obj.Rect, _rect);
    }
    public void Draw()
    {
        EG_Debug.DrawRect(_rect, DRAW_QUAD_COLOR);

        if(Time.frameCount - _lastActivityFrameNumber <= LAST_ACTIVITY_DRAW_MARGIN)
            EG_Debug.DrawRect(_rect.Shrink(0.1f), DRAW_ACTIVITY_COLOR);

        for (int i = 0; i < _objects.Count; i++)
            EG_Debug.DrawRect(_objects[i].Rect, DRAW_OBJECT_COLOR);

        if (HasChildNodes)
        {
            _childNodes.Do(x => x.Draw());
        }
    }
    public List<T> Query(Rect rect)
    {
        PollActivity();
        
        List<T> results = new List<T>();

        if (HasObjects)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if (Utility.Intersects(_objects[i].Rect, rect) || Utility.Encapsulates(_objects[i].Rect, rect))
                {
                    results.Add(_objects[i].Object);
                }
            }
        }

        if (HasChildNodes)
        {
            if (Utility.Encapsulates(_rect, rect))
            {
                results.AddRange(GetObjectsRecursively());
            }
            else if (Utility.Intersects(_rect, rect))
            {
                for (int i = 0; i < _childNodes.Count; i++)
                {
                    results.AddRange(_childNodes[i].Query(rect));
                }
            }
        }
        
        return results;
    }
    private List<T> GetObjectsRecursively()
    {
        PollActivity();

        List<T> results = new List<T>();

        if(HasObjects)
        {
            results.AddRange(_objects.Select(x => x.Object));
        }

        if (HasChildNodes)
        {
            for (int i = 0; i < _childNodes.Count; i++)
            {
                results.AddRange(_childNodes[i].GetObjectsRecursively());
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
