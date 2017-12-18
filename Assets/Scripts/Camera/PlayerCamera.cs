using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour {

    [SerializeField]
    private float ratio;

    private bool isScrollWheelDown;
    private Vector3 mouseDelta;
    private Vector3 oldMousePosition;
    private Vector3 keyboardMovementDirection;
    private float scrollWheelDelta;
    private bool isShiftDown;
    private Vector3 scrollWheelDownWorldPos;

    private const float CAMERA_DRAG_SPEED = 1;
    private const float ZOOM_SPEED = 200;
    private const float KEYBOARD_MOVEMENT_SPEED = 30;
    private const float KEYBOARD_MOVEMENT_SHIFT_SPEED = 50;

    private const float ORTHOGRAPHIC_MIN = 10;
    private const float ORTHOGRAPHIC_MAX = 50;

    private static new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();

        camera.orthographicSize = ORTHOGRAPHIC_MIN + ((float)(ORTHOGRAPHIC_MAX - ORTHOGRAPHIC_MIN) / 2);
        ratio = camera.aspect;
    }
	private void Update()
    {
        PollInput();
        ProcessInput();

        camera.aspect = ratio;
    }
    public static void Center()
    {
        IEnumerable<Entity> colonists = MapData.Entities.Where(x => x.GetType() == typeof(Colonist));

        Vector3 cumulativePosition = Vector3.zero;

        foreach (Entity entity in colonists)
        {
            cumulativePosition += entity.transform.position;
        }

        camera.transform.position = new Vector2()
        {
            x = cumulativePosition.x / colonists.Count(),
            y = cumulativePosition.y / colonists.Count(),
        };
    }
    private void PollInput()
    {
        mouseDelta = oldMousePosition - Input.mousePosition;
        oldMousePosition = Input.mousePosition;

        isScrollWheelDown = Input.GetKey(KeyCode.Mouse2);
        isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        scrollWheelDelta = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            scrollWheelDownWorldPos = MouseToWorldPoint();
        }
        else if (Input.GetKey(KeyCode.Mouse2))
        {
            mouseDelta = scrollWheelDownWorldPos - MouseToWorldPoint();
        }


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
    private Vector3 MouseToWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance = -(Camera.main.transform.position.z - Player.Instance.transform.position.z);

        return ray.origin + ray.direction * distance;
    }
    private void ProcessInput()
    {
        if (isScrollWheelDown)
        {
            transform.position += mouseDelta;
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
