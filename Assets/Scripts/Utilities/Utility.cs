using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {
    
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
