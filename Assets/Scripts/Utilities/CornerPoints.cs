using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CornerPoints {

    public CornerPoints(Rect rect)
    {
        points = Utility.GetCornerPoints(rect.center, rect.size);
    }
    public CornerPoints(Vector2[] points)
    {
        if (points.Length != 4)
            throw new System.ArgumentException("Points array must be exactly 4!");

        this.points = points;
    }

	public Vector2 this[int i]
    {
        get
        {
            return points[i];
        }
        set
        {
            points[i] = value;
        }
    }

    public Vector2 Size { get { return TopRight - BottomLeft; } }

    private Vector2[] points;

    private Vector2 TopLeft { get { return points[0]; } }
    private Vector2 TopRight { get { return points[1]; } }
    private Vector2 BottomRight { get { return points[2]; } }
    private Vector2 BottomLeft { get { return points[3]; } }

    public void Transform(System.Func<Vector2, Vector2> function)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = function(points[i]);
        }
    }

    public static implicit operator Vector2[](CornerPoints cornerPoints)
    {
        return cornerPoints.points;
    }
    public static explicit operator Rect(CornerPoints cornerPoints)
    {
        return new Rect(cornerPoints.TopLeft, cornerPoints.TopRight - cornerPoints.BottomLeft);
    }
    public static explicit operator CornerPoints(Vector2[] points)
    {
        return new CornerPoints(points);
    }    
}
