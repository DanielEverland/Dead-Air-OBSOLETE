using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareFormation : IUnitFormation
{
    private PriorityQueue<Vector2, float> priorityQueue = new PriorityQueue<Vector2, float>();

    public IEnumerable<Vector2> GetPattern(int unitCount)
    {
        priorityQueue.Clear();

        int radius = Mathf.CeilToInt(Mathf.Sqrt(unitCount) / 2);

        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                priorityQueue.Enqueue(new Vector2(x, y), GetPriority(x, y));
            }
        }

        List<Vector2> pattern = new List<Vector2>();

        for (int i = 0; i < unitCount; i++)
        {
            pattern.Add(priorityQueue.Dequeue());
        }

        return pattern;
    }
    private float GetPriority(int x, int y)
    {
        return Mathf.Abs(x) + Mathf.Abs(y * y);
    }
}
