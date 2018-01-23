using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EG_GL : MonoBehaviour
{
    private Camera glCamera;

    private static Material _currentMaterial;

    private static List<short> RECT_REMAINING_INDEXES = new List<short>();

    private static Rect[] RECT_OBJECT_BUFFER = new Rect[short.MaxValue];
    private static Color[] RECT_COLOR_BUFFER = new Color[short.MaxValue];
    private static float[] RECT_DURATION_BUFFER = new float[short.MaxValue];
    private static short RECT_INDEX = -1;

    private void Awake()
    {
        _currentMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        
        glCamera = GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        glCamera.orthographicSize = Camera.main.orthographicSize;
    }
    public static void DrawRect(Rect rect, Color color, float duration)
    {
        RECT_INDEX++;

        RECT_OBJECT_BUFFER[RECT_INDEX] = rect;
        RECT_COLOR_BUFFER[RECT_INDEX] = color;
        RECT_DURATION_BUFFER[RECT_INDEX] = duration;
    }
    private void OnPostRender()
    {
        GL.PushMatrix();
        _currentMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        for (short i = 0; i < RECT_INDEX; i++)
        {
            DrawRect(i);
        }
        GL.End();

        GL.PopMatrix();

        Clear();
        AddRemainingObjects();
    }
    private void DrawRect(short index)
    {
        GL.Color(RECT_COLOR_BUFFER[index]);

        Rect rect = RECT_OBJECT_BUFFER[index];
        Debug.Log(rect);

        //TOP
        GL.Vertex(rect.min);
        GL.Vertex(new Vector2(rect.xMin + rect.width, rect.yMin));

        //RIGHT
        GL.Vertex(new Vector2(rect.xMin + rect.width, rect.yMin));
        GL.Vertex(rect.max);

        //BOTTOM
        GL.Vertex(new Vector3(rect.xMin, rect.yMin + rect.height));
        GL.Vertex(rect.max);

        //LEFT
        GL.Vertex(rect.min);
        GL.Vertex(new Vector3(rect.xMin, rect.yMin + rect.height));

        RECT_DURATION_BUFFER[index] -= Time.unscaledDeltaTime;

        if (RECT_DURATION_BUFFER[index] > 0)
            RECT_REMAINING_INDEXES.Add(index);
    }
    private void Clear()
    {
        RECT_INDEX = -1;
    }
    private void AddRemainingObjects()
    {
        while (RECT_REMAINING_INDEXES.Count > 0)
        {
            RECT_INDEX++;

            if (RECT_REMAINING_INDEXES.Contains(RECT_INDEX))
            {
                if (RECT_INDEX == RECT_REMAINING_INDEXES[0])
                    RECT_REMAINING_INDEXES.RemoveAt(0);

                break;
            }

            RECT_OBJECT_BUFFER[RECT_INDEX] = RECT_OBJECT_BUFFER[RECT_REMAINING_INDEXES[0]];
            RECT_COLOR_BUFFER[RECT_INDEX] = RECT_COLOR_BUFFER[RECT_REMAINING_INDEXES[0]];
            RECT_DURATION_BUFFER[RECT_INDEX] = RECT_DURATION_BUFFER[RECT_REMAINING_INDEXES[0]];

            RECT_REMAINING_INDEXES.RemoveAt(0);
        }
    }

    public enum GL_Materials
    {
        Unlit,
    }
}