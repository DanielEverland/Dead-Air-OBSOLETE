using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour {

    private static DebugMenu _menuInstance;

    [SerializeField]
    private DebugToggleElement _toggleElementPrefab;
    [SerializeField]
    private RectTransform _categoryRoot;
    [SerializeField]
    private RectTransform _contentRoot;
    [SerializeField]
    private DebugMenuCategoryElement _debugMenuElementPrefab;
    [SerializeField]
    private DebugMenuCategory _categoryPrefab;

    private Dictionary<byte, List<DebugManager.AttributeEntry>> _categorisedData = new Dictionary<byte, List<DebugManager.AttributeEntry>>();
    private Dictionary<byte, GameObject> _contentObjects = new Dictionary<byte, GameObject>();
    private Dictionary<string, byte> _idLookup = new Dictionary<string, byte>();

    private GameObject _currentContent;
        
    private void Awake()
    {
        _menuInstance = this;

        gameObject.SetActive(false);
    }
    private void Start()
    {
        Initialize();
        CreateCategoryMenu();
    }
    private void Initialize()
    {
        foreach (DebugManager.AttributeEntry entry in DebugManager.Attributes)
        {
            LoadEntry(entry);
        }
    }
    private void CreateCategoryMenu()
    {
        foreach (KeyValuePair<string, byte> keyValuePair in _idLookup)
        {
            DebugMenuCategoryElement categoryElement = Instantiate(_debugMenuElementPrefab);
            categoryElement.transform.SetParent(_categoryRoot); 

            categoryElement.Initialize(this, keyValuePair.Value, keyValuePair.Key);
        }
    }
    public void CategorySelected(byte ID)
    {
        if (!_contentObjects.ContainsKey(ID))
            CreateContent(ID);

        if (_currentContent != null)
            _currentContent.SetActive(false);

        _currentContent = _contentObjects[ID];
        _currentContent.SetActive(true);
    }
    private void CreateContent(byte ID)
    {
        DebugMenuCategory category = Instantiate(_categoryPrefab);
        category.transform.SetParent(_contentRoot, false);
        _contentObjects.Add(ID, category.gameObject);

        category.Initialize(_categorisedData[ID]);
    }
    private void LoadEntry(DebugManager.AttributeEntry entry)
    {
        if (!_idLookup.ContainsKey(entry.Attribute.Category))
        {
            _categorisedData.Add((byte)_idLookup.Count, new List<DebugManager.AttributeEntry>());
            _idLookup.Add(entry.Attribute.Category, (byte)_idLookup.Count);
        }

        byte id = _idLookup[entry.Attribute.Category];
        _categorisedData[id].Add(entry);
    }
    private void AddProperty(DebugManager.PropertyAttributeEntry propertyEntry)
    {
        if(propertyEntry.Attribute is EG_Debug.Toggle)
        {
            AddToggle(propertyEntry.Attribute as EG_Debug.Toggle, propertyEntry.Property);
        }
        else
        {
            Debug.LogError("Property " + propertyEntry.Attribute.GetType() + " is not recognized");
        }
    } 
    private void AddToggle(EG_Debug.Toggle attribute, PropertyInfo info)
    {
        DebugToggleElement toggleElement = Instantiate(_toggleElementPrefab);
        toggleElement.transform.SetParent(transform, false);

        toggleElement.Initialize(attribute, info);
    }
    private void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        transform.SetAsLastSibling();
    }
    public static void GlobalUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _menuInstance.Toggle();
        }
    }
}
