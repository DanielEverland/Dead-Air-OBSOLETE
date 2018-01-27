using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EG_GL : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _textElementPrefab;

    private Camera glCamera;

    private static Material _currentMaterial;

    private static LinkedList<DrawCall> _drawQueue = new LinkedList<DrawCall>();
    private static LinkedList<DrawCall> _toDraw = new LinkedList<DrawCall>();

    private static TMP_Text _textElement;
    
    private void Awake()
    {
        _currentMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        
        glCamera = GetComponent<Camera>();
        _textElement = Instantiate(_textElementPrefab);
        _textElement.text = "";
    }
    public static void DrawText(string text, Color color, float duration)
    {
        _toDraw.AddLast(new TextObject(text, color, duration));
    }
    public static void DrawCircle(Vector2 center, float radius, Color color, float duration, bool isFilled)
    {
        _toDraw.AddLast(new CircleObject(center, radius, isFilled, color, duration));
    }
    public static void DrawRect(Rect rect, Color color, float duration, bool isFilled)
    {
        _toDraw.AddLast(new RectObject(rect, isFilled, color, duration));
    }
    private void LateUpdate()
    {
        glCamera.orthographicSize = Camera.main.orthographicSize;
    }
    private void OnPostRender()
    {
        _textElement.text = "";

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
        public DrawCall(float duration, Color color)
        {
            _duration = duration;
            _color = color;
        }

        public float Duration { get { return _duration; } }

        protected readonly Color _color;
        
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
    private class TextObject : DrawCall
    {
        public TextObject(string text, Color color, float duration) : base(duration, color)
        {
            _text = text;
        }

        private readonly string _text;

        protected override void DoDraw()
        {
            if (_color != Color.white)
                _textElement.text += string.Format("<color=#{0}>{1}</color>\n", ColorUtility.ToHtmlStringRGBA(_color), _text);
            else
                _textElement.text += _text + "\n";
        }
    }
    private class LineObject : DrawCall
    {
        public LineObject(Vector2 a, Vector2 b, Color color, float duration) : base(duration, color)
        {
            _a = a;
            _b = b;
        }

        private readonly Vector2 _a;
        private readonly Vector2 _b;

        protected override void DoDraw()
        {
            GL.Begin(GL.LINES);
            GL.Color(_color);

            GL.Vertex(_a);
            GL.Vertex(_b);

            GL.End();
        }
    }
    private abstract class FillObject : DrawCall
    {
        public FillObject(bool isFilled, float duration, Color color) : base(duration, color)
        {
            _isFilled = isFilled;
        }

        private readonly bool _isFilled;

        protected override void DoDraw()
        {
            if (_isFilled)
                DrawFilled();
            else
                DrawOutline();
        }

        protected abstract void DrawFilled();
        protected abstract void DrawOutline();
    }
    private class CircleObject : FillObject
    {
        public CircleObject(Vector2 center, float radius, bool isFilled, Color color, float duration) : base(isFilled, duration, color)
        {
            int vertexCount = Mathf.RoundToInt(radius * VERTEX_COUNT_MULTIPLIER) + VERTEX_COUNT_ADDITIVE;

            _vertices = new List<Vector2>();
            _center = center;

            for (int i = 0; i < vertexCount; i++)
            {
                _vertices.Add(new Vector2(
                    radius * Mathf.Cos(2 * Mathf.PI * i / vertexCount) + center.x,
                    radius * Mathf.Sin(2 * Mathf.PI * i / vertexCount) + center.y));
            }
        }

        private const float VERTEX_COUNT_MULTIPLIER = 1.5f;
        private const int VERTEX_COUNT_ADDITIVE = 15;
        
        private readonly List<Vector2> _vertices;
        private readonly Vector2 _center;

        protected override void DrawFilled()
        {
            GL.Begin(GL.TRIANGLES);
            GL.Color(_color);

            for (int i = 0; i < _vertices.Count; i++)
            {
                GL.Vertex(_center);

                GL.Vertex(_vertices[i]);
                GL.Vertex(_vertices[i == 0 ? _vertices.Count - 1 : i - 1]);
            }

            GL.End();
        }
        protected override void DrawOutline()
        {
            GL.Begin(GL.LINES);
            GL.Color(_color);

            for (int i = 0; i < _vertices.Count; i++)
            {
                GL.Vertex(_vertices[i]);
                GL.Vertex(_vertices[i == 0 ? _vertices.Count - 1 : i - 1]);
            }

            GL.End();
        }
    }
    private class RectObject : FillObject
    {
        public RectObject(Rect rect, bool isFilled, Color color, float duration) : base(isFilled, duration, color)
        {
            _rect = rect;
        }

        private Rect _rect;
        
        protected override void DrawFilled()
        {
            GL.Begin(GL.QUADS);
            GL.Color(_color);

            GL.Vertex(_rect.min);
            GL.Vertex(_rect.min + new Vector2(_rect.width, 0));
            GL.Vertex(_rect.max);
            GL.Vertex(_rect.min + new Vector2(0, _rect.height));

            GL.End();
        }
        protected override void DrawOutline()
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