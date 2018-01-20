using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenuCategory : MonoBehaviour {

    [SerializeField]
    private HeaderElement _headerPrefab;
    [SerializeField]
    private RectTransform _headerContentRoot;
    [SerializeField]
    private DebugToggleElement _togglePrefab;

    private Dictionary<string, HeaderElement> _headers = new Dictionary<string, HeaderElement>();

    public void Initialize(List<DebugManager.AttributeEntry> attributes)
    {
        foreach (DebugManager.AttributeEntry attribute in attributes)
        {
            HeaderElement header = GetHeader(attribute.Attribute.Header);

            GameObject attributeObject = CreateAttribute(attribute);

            header.AddElement(attributeObject);
        }
    }
    private GameObject CreateAttribute(DebugManager.AttributeEntry attribute)
    {
        if(attribute is DebugManager.PropertyAttributeEntry)
        {
            return CreatePropertyElement(attribute as DebugManager.PropertyAttributeEntry);
        }
        else
        {
            throw new System.NotImplementedException("Cannot recognize type " + attribute.GetType());
        }
    }
    private GameObject CreatePropertyElement(DebugManager.PropertyAttributeEntry property)
    {
        if(property.Attribute is EG_Debug.Toggle)
        {
            DebugToggleElement toggleElement = Instantiate(_togglePrefab);

            toggleElement.Initialize(property.Attribute as EG_Debug.Toggle, property.Property);

            return toggleElement.gameObject;
        }
        else
        {
            throw new System.NotImplementedException("Cannot recognize type " + property.Attribute.GetType());
        }
    }
    private HeaderElement GetHeader(string headerName)
    {
        if (!_headers.ContainsKey(headerName))
        {
            CreateHeader(headerName);   
        }

        return _headers[headerName];
    }
    private void CreateHeader(string headerName)
    {
        HeaderElement headerElement = Instantiate(_headerPrefab);
        headerElement.Initialize(headerName);
        headerElement.transform.SetParent(_headerContentRoot);

        _headers.Add(headerName, headerElement);
    }
}
