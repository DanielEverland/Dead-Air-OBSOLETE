using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class World {

    public static GameObject RootObject
    {
        get
        {
            if (_rootObject == null)
                _rootObject = new GameObject("World");

            return _rootObject;
        }
    }
    private static GameObject _rootObject;

    public static GameObject Entities
    {
        get
        {
            if (_entities == null)
                _entities = CreateObject("Entities");

            return _entities;
        }
    }
    private static GameObject _entities;

    public static GameObject Terrain
    {
        get
        {
            if (_terrain == null)
                _terrain = CreateObject("Terrain");

            return _terrain;
        }
    }
    private static GameObject _terrain;

    public static GameObject Objects
    {
        get
        {
            if (_objects == null)
                _objects = CreateObject("Objects");

            return _objects;
        }
    }
    private static GameObject _objects;

    private static GameObject CreateObject(string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(RootObject.transform);

        return obj;
    }
}
