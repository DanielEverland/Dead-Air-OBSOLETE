using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareFormation : IUnitFormation
{
    private float SQUARE_ELONGATION = 2;

    public IEnumerable<Vector2> GetPattern(int unitCount)
    {
        List<Vector2> toReturn = new List<Vector2>(unitCount);

        float radius = Mathf.Sqrt(unitCount) / 2;
        int xSize = Mathf.CeilToInt(radius * SQUARE_ELONGATION);
        int ySize = Mathf.CeilToInt(radius / SQUARE_ELONGATION);

        for (int x = -xSize; x <= xSize; x++)
        {
            for (int y = -ySize; y <= ySize; y++)
            {
                toReturn.Add(new Vector2(x, y));
            }
        }

        return toReturn;
    }
}
