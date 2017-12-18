using System.Collections.Generic;
using UnityEngine;

public interface IUnitFormation {

    IEnumerable<Vector2> GetPattern(int unitCount);
}
