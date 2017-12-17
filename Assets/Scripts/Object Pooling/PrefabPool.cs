using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool : GenericObjectPoolBehaviour {

    private static GenericObjectPoolBehaviour instance;

    protected override void Awake()
    {
        base.Awake();

        instance = this;
    }
    public new static GameObject GetObject(string key)
    {
        return (GameObject)instance.GetObject(key);
    }
    public static void ReturnObject(GameObject obj)
    {
        instance.ReturnObject(obj);
    }
}
