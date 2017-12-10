using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

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

    public abstract string PrefabName { get; }
}
