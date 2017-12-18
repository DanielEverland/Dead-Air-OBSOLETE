using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitFormation {
    
    private static Dictionary<System.Type, IUnitFormation> formations = new Dictionary<System.Type, IUnitFormation>
    {
        { typeof(SquareFormation), new SquareFormation() },

    };

    public static IEnumerable<Vector2> GetFormation<T>(Vector2 anchorPosition, int unitCount) where T : IUnitFormation
    {
        return formations[typeof(T)].GetPattern(unitCount).Select(x => x + anchorPosition);
    }
}
