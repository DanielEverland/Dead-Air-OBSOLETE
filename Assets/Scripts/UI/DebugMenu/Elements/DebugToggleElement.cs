using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Toggle))]
public class DebugToggleElement : MonoBehaviour {

    [SerializeField]
    private Toggle _toggleElement;
    [SerializeField]
    private TMP_Text _textElement;

    private PropertyInfo _propertyInfo;

    public void Initialize(EG_Debug.Toggle toggle, PropertyInfo info)
    {
        this._propertyInfo = info;

        _toggleElement.onValueChanged.AddListener(OnValueChanged);
        _toggleElement.isOn = toggle.DefaultValue;
        _textElement.text = toggle.Title;
    }
    private void OnValueChanged(bool value)
    {
        _propertyInfo.SetValue(null, value, null);
    }
}
