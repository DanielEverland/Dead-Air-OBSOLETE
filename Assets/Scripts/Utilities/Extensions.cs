using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector3 topLeft = corners[0];

        return new Rect(topLeft, rectTransform.rect.size);
    }
    /// <summary>
    /// Determines whether <paramref name="a"/> is divisible by <paramref name="b"/>
    /// </summary>
    public static bool IsDivisible(this int a, float b)
    {
        return !((float)a / b).IsDecimal();
    }
    /// <summary>
    /// Determines whether <paramref name="a"/> is divisible by <paramref name="b"/>
    /// </summary>
    public static bool IsDivisible(this float a, float b)
    {
        return !(a / b).IsDecimal();
    }
    public static List<System.Type> GetDerivedTypesOrdered(this System.Type type)
    {
        List<System.Type> unordered = GetDerivedTypes(type);
        System.Func<System.Type, int> test = x =>
        {
            int toReturn = 0;

            for (int i = 0; i < unordered.Count; i++)
            {
                if (unordered[i].IsAssignableFrom(x))
                {
                    toReturn++;
                }
            }

            return toReturn;
        };

        return GetDerivedTypes(type).OrderBy(test).ToList();
    }
    public static List<System.Type> GetDerivedTypes(this object obj)
    {
        return obj.GetType().GetDerivedTypes();
    }
    public static List<System.Type> GetDerivedTypes(this System.Type type)
    {
        return (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where type.IsSubclassOf(assemblyType)
                select assemblyType).ToList();
    }
    public static Vector2 ToCellPosition(this Vector3 position)
    {
        return new Vector2()
        {
            x = Mathf.RoundToInt(position.x),
            y = Mathf.RoundToInt(position.y),
        };
    }
    public static Vector2 ToCellPosition(this Vector2 position)
    {
        return new Vector2()
        {
            x = Mathf.RoundToInt(position.x),
            y = Mathf.RoundToInt(position.y),
        };
    }
    public static void ForEach<T>(this IEnumerable<T> enumeration, System.Action<T> action)
    {
        foreach (T item in enumeration)
        {
            action(item);
        }
    }
    public static Vector2[] GetCornerPoints(this Rect rect)
    {
        return Utility.GetCornerPoints(rect.center, rect.size);
    }
    /// <summary>
    /// Returns a new rect that is shrunk by <paramref name="amount"/> on all sides 
    /// </summary>
    public static Rect Shrink(this Rect rect, float amount)
    {
        return rect.Shrink(amount, amount, amount, amount);
    }
    /// <summary>
    /// Returns a new rect that is shrunk by amounts specified 
    /// </summary>
    public static Rect Shrink(this Rect rect, float left, float top, float right, float bottom)
    {
        return new Rect(rect.x + left, rect.y + bottom, rect.width - (left + right), rect.height - (top + bottom));
    }
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
        return (x % 1) != 0;
    }
    public static T Random<T>(this IEnumerable<T> list)
    {
        return list.ElementAt(UnityEngine.Random.Range(0, list.Count() - 1));
    }
}
