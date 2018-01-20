using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ListElement : Selectable {

    public bool IsOn
    {
        get
        {
            return _isOn;
        }
        set
        {
            if(value == true)
            {
                Select();
            }
            else
            {
                Deselect();
            }
        }
    }

    [SerializeField]
    private TMP_Text _textElement;
    [SerializeField]
    private ColorBlock _textColorBlock = ColorBlock.defaultColorBlock;

    [SerializeField]
    private ListElementEvent OnSelected;
    [SerializeField]
    private ListElementEvent OnDeselected;

    private ListManager ListManager
    {
        get
        {
            if (_listManager == null)
                CheckForListManager(transform.parent);

            return _listManager;
        }
    }

    private ListManager _listManager;
    private bool _isOn;
    private bool _containsMouse;
    
    protected override void Start()
    {
        base.Start();

        ListManager.RegisterElement(this);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        Select();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        _containsMouse = true;

        if (!_isOn)
        {
            DoStateTransition(SelectionState.Highlighted, false);
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        _containsMouse = false;

        if (!_isOn)
        {
            DoStateTransition(SelectionState.Normal, false);
        }
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        
    }
    protected override void OnDisable()
    {
        
    }
    protected override void OnEnable()
    {
        if (IsOn)
        {
            DoStateTransition(SelectionState.Pressed, true);
        }
        else
        {
            if (_containsMouse)
            {
                DoStateTransition(SelectionState.Highlighted, true);
            }
            else
            {
                DoStateTransition(SelectionState.Normal, true);
            }            
        }        
    }    
    protected new void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        if(_textElement != null)
        {
            Color tintColor;

            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = _textColorBlock.normalColor;
                    break;
                case SelectionState.Highlighted:
                    tintColor = _textColorBlock.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    tintColor = _textColorBlock.pressedColor;
                    break;
                case SelectionState.Disabled:
                    tintColor = _textColorBlock.disabledColor;
                    break;
                default:
                    tintColor = Color.black;
                    break;
            }

            _textElement.CrossFadeColor(tintColor, instant ? 0 : _textColorBlock.fadeDuration, true, true);
        }
    }
    public new void Select()
    {
        ListManager.SetAsActive(this);

        _isOn = true;

        DoStateTransition(SelectionState.Pressed, true);

        if (OnSelected != null)
            OnSelected.Invoke();
    }
    public void Deselect()
    {
        _isOn = false;

        if (_containsMouse)
        {
            DoStateTransition(SelectionState.Highlighted, false);
        }
        else
        {
            DoStateTransition(SelectionState.Normal, false);
        }

        if (OnDeselected != null)
            OnDeselected.Invoke();
    }
    private bool CheckForListManager(Transform transform)
    {
        ListManager listManager = transform.GetComponent<ListManager>();

        if(listManager != null)
        {
            _listManager = listManager;
            ListManager.RegisterElement(this);

            return true;
        }
        else
        {
            foreach (Transform child in transform)
            {
                if (CheckForListManager(child))
                    return true;
            }
        }

        return false;
    }
    [System.Serializable]
    public class ListElementEvent : UnityEvent
    {

    }
}
