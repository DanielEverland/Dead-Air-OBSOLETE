﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ListElement))]
public class DebugMenuCategoryElement : MonoBehaviour {

    [SerializeField]
    private TMP_Text textElement;
    [SerializeField]
    private ListElement listElement;

    private DebugMenu _menu;
    private byte _ID;

    public void Initialize(DebugMenu menu, byte id, string categoryName)
    {
        _menu = menu;
        _ID = id;

        textElement.text = categoryName;
    }
    public void Select()
    {
        listElement.Select();
    }
	public void OnSelected()
    {
        _menu.CategorySelected(_ID);
    }
}
