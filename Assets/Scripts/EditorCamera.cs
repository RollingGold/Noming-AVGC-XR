using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class EditorCamera : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float sprintMultiplier = 3f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.15f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.03f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 80f;

    [Header("Pan")]
    [SerializeField] private float panSpeed = 0.02f;

    [Header("Pitch")]
    [SerializeField] private float minPitch = 15f;
    [SerializeField] private float maxPitch = 80f;

    private InputSystem_Actions input;

    private Camera cam;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float zoomInput;

    private bool orbitPressed;
    private bool panPressed;
    private bool sprintPressed;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        input = new InputSystem_Actions();

        Vector3 euler = transform.rotation.eulerAngles;

        pitch = euler.x;
        yaw = euler.y;
    }

    private void OnEnable()
    {
        input.Enable();

        input.Editor.Move.performed +=
            ctx => moveInput = ctx.ReadValue<Vector2>();

        input.Editor.Move.canceled +=
            ctx => moveInput = Vector2.zero;

        input.Editor.Look.performed +=
            ctx => lookInput = ctx.ReadValue<Vector2>();

        input.Editor.Look.canceled +=
            ctx => lookInput = Vector2.zero;

        input.Editor.Zoom.performed +=
            ctx => zoomInput = ctx.ReadValue<float>();

        input.Editor.Zoom.canceled +=
            ctx => zoomInput = 0;

        input.Editor.Orbit.performed +=
            ctx => orbitPressed = true;

        input.Editor.Orbit.canceled +=
            ctx => orbitPressed = false;

        input.Editor.Pan.performed +=
            ctx => panPressed = true;

        input.Editor.Pan.canceled +=
            ctx => panPressed = false;

        input.Editor.Sprint.performed +=
            ctx => sprintPressed = true;

        input.Editor.Sprint.canceled +=
            ctx => sprintPressed = false;
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        KeyboardMovement();

        if (panPressed)
            Pan();

        if (orbitPressed)
            Orbit();

        Zoom();
    }

    #region Movement

    private void KeyboardMovement()
    {
        float speed = moveSpeed;

        if (sprintPressed)
            speed *= sprintMultiplier;

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        transform.position +=
            (forward * moveInput.y +
             right * moveInput.x) *
            speed *
            Time.deltaTime;
    }

    #endregion

    #region Pan

    private void Pan()
    {
        Vector3 movement =
            (-transform.right * lookInput.x -
             transform.up * lookInput.y) *
            panSpeed;

        transform.position += movement;
    }

    #endregion

    #region Orbit

    private void Orbit()
    {
        if (!GetLookPoint(out Vector3 pivot))
            return;

        yaw += lookInput.x * rotationSpeed;
        pitch -= lookInput.y * rotationSpeed;

        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch);

        float distance =
            Vector3.Distance(
                transform.position,
                pivot);

        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0);

        transform.position =
            pivot -
            rotation *
            Vector3.forward *
            distance;

        transform.LookAt(pivot);
    }

    #endregion

    #region Zoom

    private void Zoom()
    {
        if (Mathf.Abs(zoomInput) < 0.01f)
            return;

        if (!GetLookPoint(out Vector3 pivot))
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                pivot);

        distance -=
            zoomInput *
            zoomSpeed;

        distance = Mathf.Clamp(
            distance,
            minDistance,
            maxDistance);

        transform.position =
            pivot -
            transform.forward *
            distance;
    }

    #endregion

    #region Helpers

    private bool GetLookPoint(out Vector3 point)
    {
        Plane plane =
            new Plane(
                Vector3.up,
                Vector3.zero);

        Ray ray =
            new Ray(
                transform.position,
                transform.forward);

        if (plane.Raycast(ray, out float enter))
        {
            point = ray.GetPoint(enter);
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    #endregion
}