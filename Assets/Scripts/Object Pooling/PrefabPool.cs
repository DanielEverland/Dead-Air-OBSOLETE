using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool : GenericObjectPoolBehaviour {

    private static GenericObjectPoolBehaviour instance;

    protected override Transform RootTransform
    {
        get
        {
            return World.Objects.transform;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        instance = this;
    }
    public new static GameObject GetObject(string key)
    {
        return (GameObject)instance.GetObject(key);
    }
    public static T GetObject<T>(string key) where T : Object
    {
        return (T)instance.GetObject(key);
    }
    public new static void ReturnObject(GameObject obj)
    {
        instance.ReturnObject(obj);
    }
}
