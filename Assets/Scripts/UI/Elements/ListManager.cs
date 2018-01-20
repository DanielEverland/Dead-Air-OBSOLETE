using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListManager : MonoBehaviour {

    private List<ListElement> _listElements = new List<ListElement>();

    private ListElement _currentlyActive;

    private void Start()
    {
        if (_listElements.Count > 0)
            SetAsActive(_listElements[0]);
    }
    public void SetAsActive(ListElement element)
    {
        if(_currentlyActive != null)
        {
            if (_currentlyActive != element)
                _currentlyActive.Deselect();
        }

        if(_currentlyActive != element)
        {
            _currentlyActive = element;
            _currentlyActive.Select();
        }        
    }
	public void RegisterElement(ListElement element)
    {
        _listElements.Add(element);
    }
}
