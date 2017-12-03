using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour {

    private bool isScrollWheelDown;
    private Vector3 mouseDelta;
    private Vector3 oldMousePosition;
    private Vector3 keyboardMovementDirection;
    private float scrollWheelDelta;
    private bool isShiftDown;

    private const float CAMERA_DRAG_SPEED = 1;
    private const float ZOOM_SPEED = 200;
    private const float KEYBOARD_MOVEMENT_SPEED = 30;
    private const float KEYBOARD_MOVEMENT_SHIFT_SPEED = 50;

    private const float ORTHOGRAPHIC_MIN = 10;
    private const float ORTHOGRAPHIC_MAX = 50;

    private new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
	private void Update()
    {
        PollInput();
        ProcessInput();
    }
    private void PollInput()
    {
        mouseDelta = oldMousePosition - Input.mousePosition;
        oldMousePosition = Input.mousePosition;

        isScrollWheelDown = Input.GetKey(KeyCode.Mouse2);
        scrollWheelDelta = Input.GetAxis("Mouse ScrollWheel");
        isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);


        keyboardMovementDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            keyboardMovementDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            keyboardMovementDirection += Vector3.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            keyboardMovementDirection += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            keyboardMovementDirection += Vector3.down;
        }
    }
    private void ProcessInput()
    {
        if (isScrollWheelDown)
        {
            transform.position += mouseDelta * (CAMERA_DRAG_SPEED * Time.deltaTime);
        }
        else
        {
            float speedCoefficient = isShiftDown ? KEYBOARD_MOVEMENT_SHIFT_SPEED : KEYBOARD_MOVEMENT_SPEED;

            transform.position += keyboardMovementDirection * (speedCoefficient * Time.deltaTime);
        }

        float preferredSize = camera.orthographicSize + scrollWheelDelta * (ZOOM_SPEED * Time.deltaTime);
        
        camera.orthographicSize = Mathf.Clamp(preferredSize, ORTHOGRAPHIC_MIN, ORTHOGRAPHIC_MAX);
    }
}
