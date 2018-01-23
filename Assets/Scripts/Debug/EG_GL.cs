using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EG_GL : MonoBehaviour
{
    private Camera glCamera;

    private static Material _currentMaterial;

    private static LinkedList<DrawCall> _drawQueue = new LinkedList<DrawCall>();
    private static LinkedList<DrawCall> _toDraw = new LinkedList<DrawCall>();

    private void Awake()
    {
        _currentMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        
        glCamera = GetComponent<Camera>();
    }
    public static void DrawRect(Rect rect, Color color, float duration, bool isFilled)
    {
        _toDraw.AddLast(new RectObject(rect, color, isFilled, duration));
    }
    private void LateUpdate()
    {
        glCamera.orthographicSize = Camera.main.orthographicSize;
    }
    private void OnPostRender()
    {
        GL.PushMatrix();
        _currentMaterial.SetPass(0);

        foreach (DrawCall drawCall in _toDraw)
        {
            drawCall.Draw();
        }
        
        GL.PopMatrix();

        _toDraw = new LinkedList<DrawCall>(_drawQueue);
        _drawQueue.Clear();
    }

    private abstract class DrawCall
    {
        public DrawCall(float duration)
        {
            _duration = duration;
        }

        public float Duration { get { return _duration; } }

        private float _duration;

        public void Draw()
        {
            DoDraw();

            _duration -= Time.unscaledDeltaTime;

            if(_duration > 0)
                _drawQueue.AddLast(this);
        }
        protected abstract void DoDraw();
    }
    private class RectObject : DrawCall
    {
        public RectObject(Rect rect, Color color, bool isFilled, float duration) : base(duration)
        {
            _rect = rect;
            _color = color;
            _isFilled = isFilled;
        }

        private Rect _rect;
        private Color _color;
        private bool _isFilled;

        protected override void DoDraw()
        {
            if (_isFilled)
                DrawFilled();
            else
                DrawOutline();
        }
        private void DrawFilled()
        {
            GL.Begin(GL.QUADS);
            GL.Color(_color);

            GL.Vertex(_rect.min);
            GL.Vertex(_rect.min + new Vector2(_rect.width, 0));
            GL.Vertex(_rect.max);
            GL.Vertex(_rect.min + new Vector2(0, _rect.height));

            GL.End();
        }
        private void DrawOutline()
        {
            GL.Begin(GL.LINES);
            GL.Color(_color);

            //Top
            GL.Vertex(_rect.min);
            GL.Vertex(_rect.min + new Vector2(_rect.width, 0));

            //Right
            GL.Vertex(_rect.min + new Vector2(_rect.width, 0));
            GL.Vertex(_rect.max);

            //Bottom
            GL.Vertex(_rect.min + new Vector2(0, _rect.height));
            GL.Vertex(_rect.max);

            //Left
            GL.Vertex(_rect.min);
            GL.Vertex(_rect.min + new Vector2(0, _rect.height));

            GL.End();
        }
    }
}