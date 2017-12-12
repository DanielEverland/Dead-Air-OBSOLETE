using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
    
    /// <summary>
    /// Returns the bottom right quadrant
    /// </summary>
    public static Rect GetFourthQuadrant(this Rect rect)
    {
        return new Rect(rect.x + rect.width / 2, rect.y + rect.height / 2, rect.width / 2, rect.height / 2);
    }
    /// <summary>
    /// Returns the bottom left quadrant
    /// </summary>
    public static Rect GetThirdQuadrant(this Rect rect)
    {
        return new Rect(rect.x, rect.y + rect.height / 2, rect.width / 2, rect.height / 2);
    }
    /// <summary>
    /// Returns the top right quadrant
    /// </summary>
    public static Rect GetSecondQuadrant(this Rect rect)
    {
        return new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, rect.height / 2);
    }
    /// <summary>
    /// Returns the top left quadrant
    /// </summary>
    public static Rect GetFirstQuadrant(this Rect rect)
    {
        return new Rect(rect.x, rect.y, rect.width / 2, rect.height / 2);
    }
    public static bool IsPowerOfTwo(this byte x)
    {
        return IsPowerOfTwo((long)x);
    }
    public static bool IsPowerOfTwo(this ushort x)
    {
        return IsPowerOfTwo((ulong)x);
    }
    public static bool IsPowerOfTwo(this short x)
    {
        return IsPowerOfTwo((long)x);
    }
    public static bool IsPowerOfTwo(this float x)
    {
        return !x.IsDecimal() && IsPowerOfTwo((long)x);
    }
    public static bool IsPowerOfTwo(this uint x)
    {
        return IsPowerOfTwo((ulong)x);
    }
    public static bool IsPowerOfTwo(this int x)
    {
        return IsPowerOfTwo((long)x);
    }
    public static bool IsPowerOfTwo(this double x)
    {
        return !x.IsDecimal() && IsPowerOfTwo((long)x);
    }
    public static bool IsPowerOfTwo(this ulong x)
    {
        return x != 0 && (x & (x - 1)) == 0;
    }
    public static bool IsPowerOfTwo(this long x)
    {
        return x > 0 && (x & (x - 1)) == 0;
    }
    public static bool IsDecimal(this float x)
    {
        return IsDecimal((double)x);
    }
    public static bool IsDecimal(this double x)
    {
        return (x % 1) == 0;
    }
	public static T Random<T>(this IEnumerable<T> list)
    {
        return list.ElementAt(UnityEngine.Random.Range(0, list.Count() - 1));
    }
}
