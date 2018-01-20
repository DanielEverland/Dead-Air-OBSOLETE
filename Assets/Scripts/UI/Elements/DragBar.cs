using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBar : MonoBehaviour {

    [SerializeField]
    private RectTransform rootObject;

    private bool IsWithinRect;
    private bool IsMouseDown;

    private Vector2 oldMousePos;

    private RectTransform rectTransform { get { return (RectTransform)transform; } }

    private void Update()
    {
        PollInput();
        ProcessInput();

        oldMousePos = Input.mousePosition;
    }
    private void PollInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            IsWithinRect = rectTransform.GetWorldRect().Contains(Input.mousePosition);
        }

        IsMouseDown = Input.GetKey(KeyCode.Mouse0);
    }
    private void ProcessInput()
    {
        if(IsMouseDown && IsWithinRect)
        {
            Vector2 delta = (Vector2)Input.mousePosition - oldMousePos;

            rootObject.anchoredPosition += delta;
        }
    }
    private void OnValidate()
    {
        if (rootObject == null)
            rootObject = rectTransform;
    }
}
