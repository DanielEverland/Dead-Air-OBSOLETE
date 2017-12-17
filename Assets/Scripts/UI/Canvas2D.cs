using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas2D : MonoBehaviour {

    public static GameObject Instance { get; private set; }

	private void Awake()
    {
        Instance = gameObject;
    }
}
