using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisibleEntity : Entity, IVisibleEntity
{
    [SerializeField]
    private float _sizeRadius = 1;

    public float SizeRadius { get { return _sizeRadius; } }
    public Vector2 Position { get { return transform.position; } }
}
