using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadtreeNode<T> {

    private QuadtreeNode() { }
    public QuadtreeNode(Rect rect, int maxObjectCount = DEFAULT_MAX_OBJECT_COUNT)
    {
        _rect = rect;
        _objects = new List<DataEntry<T, Rect>>(maxObjectCount);
        _maxObjects = maxObjectCount;
    }

    public const int DEFAULT_MAX_OBJECT_COUNT = 3;

    private readonly Rect _rect;
    private readonly List<DataEntry<T, Rect>> _objects;
    private readonly int _maxObjects;

    private bool HasChildNodes { get { return _childNodes != null; } }
    private bool HasObjects { get { return _objects.Count > 0; } }

    private ChildNodes<T> _childNodes;
    private int _lastActivityFrameNumber;

    private const byte LAST_ACTIVITY_DRAW_MARGIN = 2;
    private readonly Color DRAW_QUAD_COLOR = Color.red;
    private readonly Color DRAW_ACTIVITY_COLOR = Color.cyan;
    private readonly Color DRAW_OBJECT_COLOR = Color.white;

    public void Insert(DataEntry<T, Rect> entry)
    {
        PollActivity();

        if (HasChildNodes)
        {
            if (_childNodes.Fits(entry))
            {
                _childNodes.Insert(entry);
            }
            else
            {
                Add(entry);
            }
        }
        else
        {
            Add(entry);
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
        List<DataEntry<T, Rect>> tempEntryList = new List<DataEntry<T, Rect>>(_objects);
        _objects.Clear();

        for (int i = 0; i < tempEntryList.Count; i++)
        {
            if (_childNodes.Fits(tempEntryList[i]))
            {
                _childNodes.Insert(tempEntryList[i]);
            }
            else
            {
                _objects.Add(tempEntryList[i]);
            }
        }

        PollActivity();
    }
    private void Add(DataEntry<T, Rect> entry)
    {
        _objects.Add(entry);

        PollSplit();
        PollActivity();
    }
    private void Remove(DataEntry<T, Rect> entry)
    {
        _objects.Remove(entry);

        PollActivity();
    }
    private void PollActivity()
    {
        _lastActivityFrameNumber = Time.frameCount;
    }
    public bool Fits(DataEntry<T, Rect> entry)
    {
        return Utility.Encapsulates(entry.Key, _rect);
    }
    public void Draw()
    {
        EG_Debug.DrawRect(_rect, DRAW_QUAD_COLOR);

        if(Time.frameCount - _lastActivityFrameNumber <= LAST_ACTIVITY_DRAW_MARGIN)
            EG_Debug.DrawRect(_rect.Shrink(0.1f), DRAW_ACTIVITY_COLOR);

        for (int i = 0; i < _objects.Count; i++)
            EG_Debug.DrawRect(_objects[i].Key, DRAW_OBJECT_COLOR);

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
                if (Utility.Intersects(_objects[i].Key, rect) || Utility.Encapsulates(_objects[i].Key, rect))
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

        public bool Fits(DataEntry<T, Rect> entry)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].Fits(entry))
                {
                    return true;
                }
            }

            return false;
        }
        public void Insert(DataEntry<T, Rect> entry)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].Fits(entry))
                {
                    _nodes[i].Insert(entry);
                    return;
                }
            }
            
            throw new System.ArgumentException("Rect " + entry.Key + " doesn't fit in any child nodes. Call Fits before Insert");
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
