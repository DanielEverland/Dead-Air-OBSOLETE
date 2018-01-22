using UnityEditor;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {

    public const int CONTEXT_MENU_ORDER = 100;

    //Sqrt(1^2 + 1^2)
    public const float SQUARE_CORNER_DISTANCE = 1.41421356237f;

    /// <summary>
    /// Converts radian to a vector
    /// </summary>
    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }
    /// <summary>
    /// Converts degrees to a vector
    /// </summary>
    public static Vector2 DegreesToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
    /// <summary>
    /// Checks whether <paramref name="a"/> and <paramref name="b"/> intersect
    /// </summary>
    public static bool Intersects(Rect a, Rect b)
    {
        return !((b.x + b.width <= a.x) ||
                (b.y + b.height <= a.y) ||
                (b.x >= a.x + a.width) ||
                (b.y >= a.y + a.height));
    }
    /// <summary>
    /// Checks whether <paramref name="a"/> is fully encapsulated by <paramref name="b"/>
    /// </summary>
    public static bool Encapsulates(Rect a, Rect b)
    {
        return
            a.x > b.x && a.x < b.x + b.width
            &&
            a.y > b.y && a.y < b.y + b.height
            &&
            a.x + a.width > b.x && a.x + a.width < b.x + b.width
            &&
            a.y + a.height > b.y && a.y + a.height < b.y + b.height;
    }
    public static Rect CornerPointsToRect(Vector2[] points)
    {
        if (points.Length != 4)
            throw new System.ArgumentException("Corner points length must be exactly 4!");

        return new Rect(points[0].x, points[0].y, points[2].x - points[0].x, points[2].y - points[0].y);
    }
    public static Vector2[] GetCornerPoints(Vector2 center, Vector2 size)
    {
        return new Vector2[4]
        {
            new Vector2(center.x - size.x / 2, center.y + size.y / 2),
            new Vector2(center.x + size.x / 2, center.y + size.y / 2),
            new Vector2(center.x + size.x / 2, center.y - size.y / 2),
            new Vector2(center.x - size.x / 2, center.y - size.y / 2),
        };
    }
    public static Chunk WorldPositionToChunk(Vector3 position)
    {
        return MapData.Chunks[WorldPositionToChunkPosition(position)];
    }
    public static Vector2 WorldPositionToLocalChunkPosition(Vector3 position)
    {
        return new Vector2(Mathf.RoundToInt(position.x % Chunk.CHUNK_SIZE), Mathf.RoundToInt(position.y % Chunk.CHUNK_SIZE));
    }
    public static Vector2 WorldPositionToChunkPosition(Vector3 position)
    {
        return new Vector2(Mathf.FloorToInt(position.x / Chunk.CHUNK_SIZE), Mathf.FloorToInt(position.y / Chunk.CHUNK_SIZE));
    }
    public static T CreateAssetAndRename<T>() where T : ScriptableObject
    {
        return CreateAssetAndRename<T>("New " + typeof(T).ToString() + ".asset");
    }
    public static T CreateAssetAndRename<T>(string fileName) where T : ScriptableObject
    {
        return CreateAssetAndRename<T>(GetCurrentlySelectedFolder(), fileName);
    }
    public static T CreateAssetAndRename<T>(string folderPath, string fileName) where T : ScriptableObject
    {
        T obj = ScriptableObject.CreateInstance<T>();

        ProjectWindowUtil.CreateAsset(obj, folderPath + "/" + fileName);
        Selection.activeObject = obj;

        return obj;
    }
    public static string GetCurrentlySelectedFolder()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "")
        {
            return "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            return path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        else
        {
            return path;
        }
    }
    /// <summary>
    /// This will create the necessary folder structure to ensure the path is valid
    /// </summary>
    /// <param name="path">Path is relative to asset folder. "Resources" would ensure "Assets/Resources" exists</param>
	public static void EnsurePathExists(string path)
    {
        if(!Directory.Exists(Application.dataPath + "/" + path))
        {
            string[] splitPath = path.Split('/', '\\');
            string currentPath = "";

            for (int i = 0; i < splitPath.Length; i++)
            {
                currentPath += splitPath[i] + "/";

                if (!Directory.Exists(Application.dataPath + "/" + currentPath))
                {
                    Debug.Log("Creating " + currentPath);

                    Directory.CreateDirectory(Application.dataPath + "/" + currentPath);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
