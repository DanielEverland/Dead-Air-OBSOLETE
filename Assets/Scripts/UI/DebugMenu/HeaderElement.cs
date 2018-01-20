using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeaderElement : MonoBehaviour {

    [SerializeField]
    private TMP_Text _titleElement;
    [SerializeField]
    private RectTransform _contentRoot;

    public void Initialize(string headerName)
    {
        _titleElement.text = headerName;
    }
    public void AddElement(GameObject obj)
    {
        obj.transform.SetParent(_contentRoot);
    }
}
