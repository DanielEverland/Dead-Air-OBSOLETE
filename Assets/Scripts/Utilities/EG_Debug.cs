using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EG_Debug {

    public static void DrawRect(Rect rect)
    {
        DrawRect(rect, Color.white);
    }
    public static void DrawRect(Rect rect, Color color)
    {
        DrawRect(rect, color, 0);
    }
    public static void DrawRect(Rect rect, Color color, float duration)
    {
        DrawRect(rect.center, rect.size, color, duration);
    }
    public static void DrawRect(Vector2 center, Vector2 size)
    {
        DrawRect(center, size, Color.white);
    }
    public static void DrawRect(Vector2 center, Vector2 size, Color color)
    {
        DrawRect(center, size, Color.white, 0);
    }
    public static void DrawRect(Vector2 center, Vector2 size, Color color, float duration)
    {
        Vector2[] corners = Utility.GetCornerPoints(center, size);

        for (int i = 0; i < corners.Length; i++)
        {
            Debug.DrawLine(
                corners[i],
                corners[i + 1 < corners.Length ? i + 1 : 0],
                color,
                duration,
                false);
        }
    }
}
