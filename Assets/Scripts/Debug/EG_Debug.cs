using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static partial class EG_Debug {

    public static void DrawText(string text)
    {
        DrawText(text, Color.white);
    }
    public static void DrawText(string text, Color color)
    {
        DrawText(text, color, 0);
    }
    public static void DrawText(string text, Color color, float duration)
    {
        EG_GL.DrawText(text, color, duration);
    }
    public static void DrawCircle(Vector2 center, float radius)
    {
        DrawCircle(center, radius, Color.white);
    }
    public static void DrawCircle(Vector2 center, float radius, Color color)
    {
        DrawCircle(center, radius, color, 0);
    }
    public static void DrawCircle(Vector2 center, float radius, Color color, float duration)
    {
        EG_GL.DrawCircle(center, radius, color, duration);
    }
    public static void DrawSquare(Vector2 center, Vector2 size)
    {
        DrawSquare(center, size, Color.white);
    }
    public static void DrawSquare(Vector2 center, Vector2 size, Color color)
    {
        DrawSquare(center, size, color, 0);
    }
    public static void DrawSquare(Vector2 center, Vector2 size, Color color, float duration)
    {
        DrawSquare(new Rect(center - size / 2, size), color, duration);
    }
    public static void DrawSquare(Rect rect)
    {
        DrawSquare(rect, Color.white);
    }
    public static void DrawSquare(Rect rect, Color color)
    {
        DrawSquare(rect, color, 0);
    }
    public static void DrawSquare(Rect rect, Color color, float duration)
    {
        EG_GL.DrawRect(rect, color, duration, true);
    }
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
        EG_GL.DrawRect(new Rect(center - size / 2, size), color, duration, false);
    }
}
