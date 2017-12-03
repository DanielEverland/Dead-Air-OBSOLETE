using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour {

    private bool isScrollWheelDown;
    private Vector3 mouseDelta;
    private Vector3 oldMousePosition;
    private float scrollWheelDelta;

    private const float CAMERA_SPEED = 1;
    private const float SCROLL_SPEED = 20;

    private const float ORTHOGRAPHIC_MIN = 5;
    private const float ORTHOGRAPHIC_MAX = 15;

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
    }
    private void ProcessInput()
    {
        if (isScrollWheelDown)
        {
            transform.position += mouseDelta * (CAMERA_SPEED * Time.deltaTime);
        }

        float preferredSize = camera.orthographicSize + scrollWheelDelta * (SCROLL_SPEED * Time.deltaTime);
        
        camera.orthographicSize = Mathf.Clamp(preferredSize, ORTHOGRAPHIC_MIN, ORTHOGRAPHIC_MAX);
    }
}
