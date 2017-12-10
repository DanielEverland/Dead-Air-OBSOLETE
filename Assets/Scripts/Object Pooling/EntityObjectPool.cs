using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityObjectPool : GenericObjectPoolBehaviour {

    private static GenericObjectPoolBehaviour instance;

	private void Awake()
    {
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
