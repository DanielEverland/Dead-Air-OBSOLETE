using System.Linq;
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
    private List<DebugElement> _elements = new List<DebugElement>();

    private void OnEnable()
    {
        SetDirty();
    }
    public void SetDirty()
    {
        _elements.ForEach(x => x.DoReload());
    }
    public void Initialize(List<DebugManager.AttributeEntry> attributes)
    {
        foreach (DebugManager.AttributeEntry attribute in attributes)
        {
            HeaderElement header = GetHeader(attribute.Attribute.Header);

            DebugElement attributeObject = CreateAttribute(attribute);
            attributeObject.Initialize(this);

            header.AddElement(attributeObject.gameObject);
            _elements.Add(attributeObject);
        }
    }
    private DebugElement CreateAttribute(DebugManager.AttributeEntry attribute)
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
    private DebugElement CreatePropertyElement(DebugManager.PropertyAttributeEntry property)
    {
        if(property.Attribute is EG_Debug.Toggle)
        {
            DebugToggleElement toggleElement = Instantiate(_togglePrefab);

            toggleElement.Initialize(property.Attribute as EG_Debug.Toggle, property.Property);

            return toggleElement;
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
