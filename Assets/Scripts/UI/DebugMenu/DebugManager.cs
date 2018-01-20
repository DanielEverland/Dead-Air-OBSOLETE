using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugManager {

    public static IEnumerable<AttributeEntry> Attributes { get { return _attributes; } }
   
    private static List<AttributeEntry> _attributes;

	public static void Initialize()
    {
        if (!Debug.isDebugBuild)
            return;

        LoadDebugSettings();
        CreateDebugMenu();
    }
    private static void CreateDebugMenu()
    {
        GameObject obj = PrefabPool.GetObject("DebugMenu");

        obj.transform.SetParent(Canvas2D.Instance.transform, false);
    }
    private static void LoadDebugSettings()
    {
        _attributes = new List<AttributeEntry>();
        
        foreach (Type type in Assembly.GetAssembly(typeof(Game)).GetTypes())
        {
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic))
            {
                PollProperty(property);
            }
        }
    }    
    private static void PollProperty(PropertyInfo property)
    {
        object[] attributes = property.GetCustomAttributes(typeof(EG_Debug.PropertyAttribute), true);

        for (int i = 0; i < attributes.Length; i++)
        {
            EG_Debug.PropertyAttribute attribute = (EG_Debug.PropertyAttribute)attributes[i];

            CreateNewPropertyEntry(attribute, property);
        }
    }
    private static void CreateNewPropertyEntry(EG_Debug.PropertyAttribute attribute, PropertyInfo property)
    {
        PropertyAttributeEntry entry = new PropertyAttributeEntry()
        {
            Attribute = attribute,
            Property = property,
        };

        _attributes.Add(entry);
    }

    public class AttributeEntry
    {
        public EG_Debug.DebugAttribute Attribute;
    }
    public class PropertyAttributeEntry : AttributeEntry
    {   
        public PropertyInfo Property;
    }
}

#region Attribute Definitions
public static partial class EG_Debug
{
    public abstract class DebugAttribute : Attribute
    {
        private DebugAttribute() { }

        public DebugAttribute(string category, string title)
        {
            _title = title;
            _category = category;
        }

        public string Title { get { return _title; } }
        public string Category { get { return _category; } }
        public string Header { get { return _header; } set { _header = value; } }

        private string _header = "";

        private readonly string _title;
        private readonly string _category;
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class PropertyAttribute : DebugAttribute
    {
        public PropertyAttribute(string category, string title) : base(category, title) { }
    }


    public class Toggle : PropertyAttribute
    {
        public Toggle(string category, string title, bool defaultValue) : base(category, title)
        {
            _defaultValue = defaultValue;
        }

        public bool DefaultValue { get { return _defaultValue; } }

        private readonly bool _defaultValue;
    }
}
#endregion