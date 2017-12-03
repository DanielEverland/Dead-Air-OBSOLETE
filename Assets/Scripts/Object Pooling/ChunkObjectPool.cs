using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkObjectPool : MonoBehaviour {

    [SerializeField]
    private int objectsToPool = 100;

    private new static Transform transform;
    private static Queue<MeshFilter> objects = new Queue<MeshFilter>();
    private static HashSet<int> temporaryObjects = new HashSet<int>();

    private void Awake()
    {
        transform = gameObject.transform;

        for (int i = 0; i < objectsToPool; i++)
        {
            ReturnObject(CreateMeshObject());
        }
    }
    private static MeshFilter CreateMeshObject()
    {
        GameObject obj = new GameObject("Chunk Object");
        obj.AddComponent<MeshRenderer>();
        MeshFilter filter = obj.AddComponent<MeshFilter>();
        
        return filter;
    }
    public static MeshFilter GetMeshFilter()
    {
        MeshFilter filter;

        if(objects.Count > 0)
        {
            filter = objects.Dequeue();
        }
        else
        {
            filter = CreateMeshObject();
            temporaryObjects.Add(filter.gameObject.GetInstanceID());

            filter.gameObject.name = "Temp";
        }

        filter.transform.SetParent(null);
        filter.gameObject.SetActive(true);

        return filter;
    }
    public static void ReturnObject(GameObject obj)
    {
        MeshFilter filter = obj.GetComponent<MeshFilter>();

        if (filter != null)
        {
            ReturnObject(filter);
        }
        else
        {
            Debug.LogError("Can't return an object without a mesh filter");
        }
    }
    public static void ReturnObject(MeshFilter filter)
    {
        if (temporaryObjects.Contains(filter.gameObject.GetInstanceID()))
        {
            Destroy(filter.gameObject);
            return;
        }

        filter.transform.SetParent(transform);
        filter.gameObject.SetActive(false);

        objects.Enqueue(filter);
    }
}
