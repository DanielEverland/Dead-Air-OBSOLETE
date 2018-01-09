using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPoolBehaviour : MonoBehaviour {

    [SerializeField]
    private GenericObjectPoolObject references;

    private Dictionary<string, Queue<Object>> pool;
    private Dictionary<string, Object> originalObjects;
    private Dictionary<int, string> objectKeys;

    protected virtual Transform RootTransform { get { return null; } }

    protected virtual void Awake()
    {
        pool = new Dictionary<string, Queue<Object>>();
        originalObjects = new Dictionary<string, Object>();
        objectKeys = new Dictionary<int, string>();

        if(references == null)
        {
            Debug.LogError("Object pool behaviour doesn't have a reference", gameObject);
            return;
        }

        for (int i = 0; i < references.Count; i++)
        {
            CreateObjects(references.GetReference(i));
        }
    }
    protected virtual void CreateObjects(GenericObjectPoolObject.ObjectReference reference)
    {
        if (pool.ContainsKey(reference.Key))
        {
            Debug.LogError("Duplicate object pool keys", gameObject);
            return;
        }

        pool.Add(reference.Key, new Queue<Object>());
        originalObjects.Add(reference.Key, reference.Object);

        for (int i = 0; i < reference.Instances; i++)
        {
            pool[reference.Key].Enqueue(CreateObject(reference.Object));
        }
    }
    protected virtual Object CreateObject(Object prefab)
    {
        Object toReturn = Instantiate(prefab);
        
        if(toReturn is GameObject)
        {
            ReturnObject(toReturn as GameObject);
        }
        else if(toReturn is MonoBehaviour)
        {
            MonoBehaviour script = toReturn as MonoBehaviour;

            ReturnObject(script.gameObject);
        }

        return toReturn;
    }
    protected virtual void ReleaseObject(Object obj)
    {
        if(obj is GameObject)
        {
            ReleaseGameObject(obj as GameObject);
        }
        else if (obj is MonoBehaviour)
        {
            MonoBehaviour script = obj as MonoBehaviour;

            ReleaseGameObject(script.gameObject);
        }
    }
    protected virtual void ReleaseGameObject(GameObject obj)
    {
        obj.transform.SetParent(RootTransform);
        obj.SetActive(true);
    }
    protected virtual void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }
    public virtual Object GetObject(string key)
    {
        if(pool[key].Count > 0)
        {
            Object obj = pool[key].Dequeue();

            objectKeys.Add(obj.GetInstanceID(), key);
            ReleaseObject(obj);

            return obj;
        }
        else
        {
            return Instantiate(originalObjects[key]);
        }
    }
    public virtual void ReturnObject(Object obj)
    {
        int instanceID = obj.GetInstanceID();

        if (objectKeys.ContainsKey(instanceID))
        {
            string key = objectKeys[instanceID];
            objectKeys.Remove(instanceID);

            if(obj is GameObject)
            {
                ReturnObject((GameObject)obj);
            }
            else if (obj is MonoBehaviour)
            {
                MonoBehaviour script = obj as MonoBehaviour;

                ReturnObject(script.gameObject);
            }

            pool[key].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}
