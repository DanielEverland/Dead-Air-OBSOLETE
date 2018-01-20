using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class DebugToggleElement : MonoBehaviour {

    [SerializeField]
    private Toggle _toggleElement;

    private PropertyInfo _propertyInfo;

    public void Initialize(EG_Debug.Toggle toggle, PropertyInfo info)
    {
        this._propertyInfo = info;

        _toggleElement.onValueChanged.AddListener(OnValueChanged);
        _toggleElement.isOn = toggle.DefaultValue;
    }
    private void OnValueChanged(bool value)
    {
        _propertyInfo.SetValue(null, value, null);
    }
}
