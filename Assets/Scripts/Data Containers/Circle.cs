using UnityEngine;

public struct Circle
{
    public Circle(Vector2 center, float radius)
    {
        this.radius = radius;
        this.center = center;
    }

    public float radius;
    public Vector2 center;

    public bool Collides(Circle other)
    {
        return Vector2.Distance(this.center, other.center) < this.radius + other.radius;
    }

    public override bool Equals(object obj)
    {
        if(obj is Circle)
        {
            Circle other = (Circle)obj;

            return other.center == this.center && other.radius == this.radius;
        }

        return false;
    }
    public override int GetHashCode()
    {
        unchecked
        {
            int i = 13;

            i *= 17 + radius.GetHashCode();
            i *= 17 + center.GetHashCode();

            return i;
        }
    }
    public override string ToString()
    {
        return string.Format("({0}, {1}) - {2})", center.x, center.y, radius);
    }
}