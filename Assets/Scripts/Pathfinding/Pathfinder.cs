using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder {

    private static Dictionary<int, Instruction> _instructions = new Dictionary<int, Instruction>();
	
    public static void MoveTo(Entity entity, Vector2 cellPosition)
    {
        if (!RegionManager.ContainsCell(cellPosition))
            return;

        Instruction instr = new Instruction(entity, cellPosition);
    }
    public static List<T> GetPath<T>(T start, T target, System.Func<T, IEnumerable<T>> getNeighbors, System.Func<T, T, T, float> getCost)
    {
        if (start == null)
            throw new System.NullReferenceException("Start cannot be null");

        if (target == null)
            throw new System.NullReferenceException("Target cannot be null");

        PriorityQueue<T, float> open = new PriorityQueue<T, float>();
        Dictionary<T, float> costs = new Dictionary<T, float>();
        Dictionary<T, T> cameFrom = new Dictionary<T, T>();
        HashSet<T> closed = new HashSet<T>();
        T current;

        open.Enqueue(start, 0);
        costs.Add(start, 0);
        cameFrom.Add(start, start);

        while (open.Count > 0)
        {
            current = open.Dequeue();

            if (current.Equals(target))
                break;
            
            foreach (T neighbor in getNeighbors(current))
            {
                float newCost = getCost(start, target, neighbor);
                
                if (costs.ContainsKey(neighbor))
                {
                    if (costs[neighbor] > newCost)
                    {
                        open.UpdatePriority(neighbor, newCost);
                        costs[neighbor] = newCost;
                        cameFrom[neighbor] = current;
                    }                        
                }
                else
                {
                    costs.Add(neighbor, newCost);
                    open.Enqueue(neighbor, newCost);
                    cameFrom.Add(neighbor, current);
                }
            }
        }

        List<T> toReturn = new List<T>();
        current = target;

        while (true)
        {
            toReturn.Add(current);

            if (current.Equals(cameFrom[current]))
                break;

            current = cameFrom[current];
        }

        toReturn.Reverse();

        return toReturn;
    }
    private static float GetCost(Vector2 start, Vector2 end, Vector2 current)
    {
        return Vector2.Distance(start, current) + Vector2.Distance(current, end);
    }
    private static float GetCost(Region start, Region end, Region current)
    {
        return GetCost(start.Position, end.Position, current.Position);        
    }
    
    private class Instruction
    {
        public Instruction(Entity entity, Vector2 targetPosition)
        {
            _entity = entity;
            _targetPosition = targetPosition;
            _targetRegion = RegionManager.GetRegion(_targetPosition);

            CreateRegionCouplings();
        }

        private readonly Entity _entity;
        private readonly Vector2 _targetPosition;
        private readonly Region _targetRegion;

        private void CreateRegionCouplings()
        {
            List<Region> RegionPath = GetPath(_entity.Region, _targetRegion, x => { return x.Neighbors; }, GetCost);

            for (int i = 0; i < RegionPath.Count; i++)
            {
                EG_Debug.DrawSquare(RegionPath[i].Bounds, Color.red, 1);
            }
        }
    }
    private struct RegionCoupling
    {
        public RegionCoupling(Region a, Region b)
        {
            _a = a;
            _b = b;
        }

        private Region _a;
        private Region _b;
    }
}
