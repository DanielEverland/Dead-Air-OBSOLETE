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

    private void Awake()
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
    private void CreateObjects(GenericObjectPoolObject.ObjectReference reference)
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
    private Object CreateObject(Object prefab)
    {
        Object toReturn = Instantiate(prefab);

        if(toReturn is GameObject)
        {
            return (toReturn as GameObject);
        }

        return toReturn;
    }
    private void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }
    public Object GetObject(string key)
    {
        if(pool[key].Count > 0)
        {
            Object obj = pool[key].Dequeue();

            objectKeys.Add(obj.GetInstanceID(), key);

            return obj;
        }
        else
        {
            return Instantiate(originalObjects[key]);
        }
    }
    public void ReturnObject(Object obj)
    {
        if (objectKeys.ContainsKey(obj.GetInstanceID()))
        {
            string key = objectKeys[obj.GetInstanceID()];

            ReturnObject(obj);
            pool[key].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}
