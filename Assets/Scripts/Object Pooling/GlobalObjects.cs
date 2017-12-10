using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObjects : GenericObjectPoolObject {

    private static GenericObjectPoolObject Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<GenericObjectPoolObject>("Global Objects");

            return instance;
        }
    }

    private static GenericObjectPoolObject instance;

    public new static T GetObject<T>(string key) where T : Object
    {
        return Instance.GetObject<T>(key);
    }
    public new static Object GetObject(string key)
    {
        return Instance.GetObject(key);
    }
}

