using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {
    
    public abstract string Name { get; }

    public byte Priority { get { return (byte)PriorityLevel; } }
    public Rect Rect { get { return new Rect(transform.position - (Vector3)Size / 2, Size); } }
    
    public virtual Vector2 Size { get { return Vector2.one; } }

    protected abstract EntityPriorityLevel PriorityLevel { get; }

    private Dictionary<string, object> data;

    public static T CreateEntity<T>() where T : Entity
    {
        return CreateEntity<T>(typeof(T).Name);
    }
    public static T CreateEntity<T>(string key) where T : Entity
    {
        GameObject obj = EntityObjectPool.GetObject(key);

        T entityReference = obj.GetComponent<T>();

        if (entityReference == null)
        {
            throw new NullReferenceException("Object " + obj.name + " doesn't have a component of type " + typeof(T).Name);
        }

        entityReference.CreateReferences();

        return entityReference;
    }	

    protected virtual void CreateReferences() { }
    
    public void Serialize()
    {
        throw new NotImplementedException("Implement data serialization here");
    }
    public void Deserialize()
    {
        throw new NotImplementedException("Implement data deserialization here");
    }
    public bool DataExists(string key)
    {
        return data.ContainsKey(key);
    }
    public void SetData(string key, object obj)
    {
        if (data.ContainsKey(key))
        {
            data[key] = obj;
        }
        else
        {
            data.Add(key, obj);
        }
    }
    public object GetData(string key)
    {
        return data[key];
    }

    public abstract string PrefabName { get; }
}
