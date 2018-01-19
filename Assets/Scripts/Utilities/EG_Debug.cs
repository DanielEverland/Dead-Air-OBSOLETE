using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EG_Debug {

    private static EG_DebugGizmos gizmosInstance;

    public static void Initialize()
    {
        GameObject gizmoObject = new GameObject("EG_Debug Gizmos");
        gizmosInstance = gizmoObject.AddComponent<EG_DebugGizmos>();
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
        gizmosInstance.AddEntry(new EG_DebugGizmos.SphereEntry(center, radius, duration, color));
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

    private class EG_DebugGizmos : MonoBehaviour
    {
        private List<Entry> entries = new List<Entry>();

        public void AddEntry(Entry entry)
        {
            entries.Add(entry);
        }
        private void OnDrawGizmos()
        {
            DrawEntries();
            PollEntries();
        }
        private void DrawEntries()
        {
            foreach (Entry entry in entries)
            {
                entry.Draw();
            }
        }
        private void PollEntries()
        {
            List<Entry> remainingEntries = new List<Entry>();

            while (entries.Count > 0)
            {
                Entry entry = entries[0];
                entries.RemoveAt(0);

                entry.timeLeft -= Time.unscaledDeltaTime;

                if (entry.timeLeft > 0)
                    remainingEntries.Add(entry);
            }

            entries = remainingEntries;
        }
        
        public class SphereEntry : Entry
        {
            public SphereEntry(Vector2 center, float radius, float duration, Color color) : base(duration, color)
            {
                this.center = center;
                this.radius = radius;
            }

            public Vector2 center;
            public float radius;

            protected override void DoDraw()
            {
                Gizmos.DrawWireSphere(center, radius);
            }
        }
        public abstract class Entry
        {
            public Entry(float duration, Color color)
            {
                this.timeLeft = duration;
                this.color = color;
            }

            public float timeLeft;
            public Color color;

            public void Draw()
            {
                Gizmos.color = color;

                DoDraw();
            }
            protected abstract void DoDraw();
        }
    }
}
