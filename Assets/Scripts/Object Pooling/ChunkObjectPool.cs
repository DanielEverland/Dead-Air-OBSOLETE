using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkObjectPool : MonoBehaviour {

    [SerializeField]
    private int objectsToPool = 200;

    private new static Transform transform;
    private static Queue<SpriteRenderer> spriteRenderers = new Queue<SpriteRenderer>();
    private static Queue<MeshFilter> meshFilters = new Queue<MeshFilter>();
    private static HashSet<int> temporaryObjects = new HashSet<int>();

    private void Awake()
    {
        transform = gameObject.transform;

        for (int i = 0; i < objectsToPool; i++)
        {
            CreateSpriteObject(i + 1);
            CreateMeshObject(i + 1);
        }
    }
    private static SpriteRenderer CreateSpriteObject(int number = -1)
    {
        string objectName = number != -1 ? "Chunk Object #" + number : "Temporary Object";

        GameObject obj = new GameObject(objectName);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();

        ReturnSpriteRenderer(renderer);

        if (number == -1)
        {
            temporaryObjects.Add(obj.GetInstanceID());
        }

        return renderer;
    }
    private static MeshFilter CreateMeshObject(int number = -1)
    {
        string objectName = number != -1 ? "Chunk Object #" + number : "Temporary Object";
        
        GameObject obj = new GameObject(objectName);
        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        MeshFilter filter = obj.AddComponent<MeshFilter>();

        ReturnMeshFilter(filter);

        if (number == -1)
        {
            temporaryObjects.Add(obj.GetInstanceID());
        }

        return filter;
    }
    public static MeshFilter GetMeshFilter()
    {
        MeshFilter filter;

        if(meshFilters.Count > 0)
        {
            filter = meshFilters.Dequeue();
        }
        else
        {
            filter = CreateMeshObject();
        }

        filter.transform.SetParent(null);
        filter.gameObject.SetActive(true);

        return filter;
    }
    public static void ReturnMeshFilter(GameObject obj)
    {
        MeshFilter filter = obj.GetComponent<MeshFilter>();

        if (filter != null)
        {
            ReturnMeshFilter(filter);
        }
        else
        {
            Debug.LogError("Can't return an object without a mesh filter");
        }
    }
    public static void ReturnMeshFilter(MeshFilter filter)
    {
        if (temporaryObjects.Contains(filter.gameObject.GetInstanceID()))
        {
            Destroy(filter.gameObject);
            return;
        }

        filter.transform.SetParent(transform);
        filter.gameObject.SetActive(false);

        meshFilters.Enqueue(filter);
    }
    public static SpriteRenderer GetSpriteRenderer()
    {
        SpriteRenderer renderer;

        if(spriteRenderers.Count > 0)
        {
            renderer = spriteRenderers.Dequeue();
        }
        else
        {
            renderer = CreateSpriteObject();
        }

        renderer.transform.SetParent(null);
        renderer.gameObject.SetActive(true);

        return renderer;
    }
    public static void ReturnSpriteRenderer(GameObject obj)
    {
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();

        if(renderer != null)
        {
            ReturnSpriteRenderer(renderer);
        }
        else
        {
            Debug.LogError("Can't return an object without a sprite renderer");
        }
    }
    public static void ReturnSpriteRenderer(SpriteRenderer renderer)
    {
        if (temporaryObjects.Contains(renderer.gameObject.GetInstanceID()))
        {
            Destroy(renderer.gameObject);
            return;
        }

        renderer.transform.SetParent(transform);
        renderer.gameObject.SetActive(false);

        spriteRenderers.Enqueue(renderer);
    }
}
