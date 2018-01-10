﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityCardBehaviour : MonoBehaviour {

    [SerializeField]
    private EntityCard.DataTypes _dataType;

    public EntityCard.DataTypes DataType { get { return _dataType; } }

    protected abstract void InitializeData(EntityCard.Data data);

    public void Initialize(EntityCard.Data data)
    {
        if (data.HasData(DataType))
        {
            Enable();

            InitializeData(data);
        }
        else
        {
            Disable();
        }
    }
    private void Enable()
    {
        gameObject.SetActive(true);
    }
    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
