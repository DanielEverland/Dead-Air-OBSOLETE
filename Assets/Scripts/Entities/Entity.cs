using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

	public static T CreateEntity<T>(T entity) where T : Entity
    {
        GameObject obj = EntityObjectPool.GetObject(entity.PrefabName);

        T entityReference = obj.GetComponent<T>();

        if (entityReference == null)
        {
            throw new NullReferenceException("Object " + obj + " doesn't have a component of type " + entity.GetType());
        }

        entityReference.CreateReferences();

        return entityReference;
    }

    protected virtual void CreateReferences() { }

    public abstract string PrefabName { get; }
}
