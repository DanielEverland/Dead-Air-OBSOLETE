using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Horde {
    
    public int Count { get { return _zombies.Count; } }

    /// <summary>
    /// Percentage that determines how many zombies must share the same work for it to be considered valid
    /// </summary>
    private const float ACCEPTABLE_WORK_PERCENTAGE = 0.8f;

    /// <summary>
    /// Time in seconds between every work check
    /// </summary>
    private const float CHECK_WORK_INTERVAL = 0.1f;

    /// <summary>
    /// Minium distance a horde should travel
    /// </summary>
    private const float MIN_DISTANCE_POSITION_DISTANCE = 30;

    private List<Zombie> _zombies;
    private float _lastWorkCheckTime;

    public Zombie this[int index]
    {
        get
        {
            return _zombies[index];
        }
        set
        {
            _zombies[index] = value;
        }
    }
    public Vector2 Position
    {
        get
        {
            return Rect.center;
        }
    }
    public Rect Rect
    {
        get
        {
            if (_rect == Rect.zero)
                SetRect();

            return _rect;
        }
        set
        {
            _rect = value;
        }
    }
    private Rect _rect;

    public void Intialize()
    {
        CheckWork();
        SetRect();
    }
    public void Add(Zombie zombie)
    {
        _zombies.Add(zombie);
    }
	public void Clear()
    {
        _zombies = new List<Zombie>();
    }
    public void Update()
    {
        SetRect();
        PollWork();
    }
    private void SetRect()
    {
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;

        for (int i = 0; i < Count; i++)
        {
            Zombie zombie = _zombies[i];

            if (zombie.Rect.xMin < minX)
                minX = zombie.Rect.xMin;

            if (zombie.Rect.xMax > maxX)
                maxX = zombie.Rect.xMax;

            if (zombie.Rect.yMin < minY)
                minY = zombie.Rect.yMin;

            if (zombie.Rect.yMax > maxY)
                maxY = zombie.Rect.yMax;
        }
                
        Rect = new Rect(minX, minY, maxX - minX, maxY - minY);
    }
    private void PollWork()
    {
        if (Time.unscaledTime - _lastWorkCheckTime >= CHECK_WORK_INTERVAL)
            CheckWork();
    }
    private void CheckWork()
    {
        _lastWorkCheckTime = Time.time;

        List<IWork> workTypes = new List<IWork>();
        List<int> workAmount = new List<int>();

        for (int i = 0; i < Count; i++)
        {
            if (workTypes.Contains(_zombies[i].CurrentWork))
            {
                int index = workTypes.IndexOf(_zombies[i].CurrentWork);

                workAmount[index]++;
            }
            else
            {
                workTypes.Add(_zombies[i].CurrentWork);
                workAmount.Add(1);
            }
        }

        IWork bestWorkObject = null;
        int bestWorkCount = 0;

        for (int i = 0; i < workTypes.Count; i++)
        {
            if(workAmount[i] > bestWorkCount && workTypes[i] != null)
            {
                bestWorkCount = workAmount[i];
                bestWorkObject = workTypes[i];
            }
        }

        float percentage = (float)bestWorkCount / (float)Count;

        if(percentage >= ACCEPTABLE_WORK_PERCENTAGE)
        {
            if (percentage == 1)
                return;

            for (int i = 0; i < Count; i++)
            {
                if(_zombies[i].CurrentWork != bestWorkObject)
                {
                    _zombies[i].AssignWork(bestWorkObject);
                }
            }
        }
        else
        {
            AssignNewWork();
        }
    }
    private void AssignNewWork()
    {
        MoveWork work = new MoveWork(GetDistantPosition());

        for (int i = 0; i < Count; i++)
        {
            _zombies[i].AssignWork(work);
        }
    }
    private Vector2 GetDistantPosition()
    {
        Vector2 randomPos = new Vector2()
        {
            x = Random.Range(0, MapData.MAPSIZE),
            y = Random.Range(0, MapData.MAPSIZE),
        };

        if(Vector2.Distance(randomPos, Position) > MIN_DISTANCE_POSITION_DISTANCE)
        {
            return randomPos;
        }
        else
        {
            return GetDistantPosition();
        }
    }

    public override bool Equals(object obj)
    {
        if(obj is Horde)
        {
            Horde other = (Horde)obj;

            if(other.Count == this.Count)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i] != other[i])
                        return false;
                }

                return true;
            }
        }

        return false;
    }
    public override int GetHashCode()
    {
        return _zombies.GetHashCode();
    }
    public override string ToString()
    {
        return base.ToString();
    }
}
