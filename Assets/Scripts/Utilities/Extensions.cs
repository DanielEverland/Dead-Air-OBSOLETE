using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

	public static T Random<T>(this IEnumerable<T> list)
    {
        return list.ElementAt(UnityEngine.Random.Range(0, list.Count() - 1));
    }
}
