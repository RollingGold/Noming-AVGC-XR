using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;

    private InputSystem_Actions inputActions;

    private Vector2 lookInput;

    private float pitch;
    private float yaw;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Look.performed += ctx =>
        {
            lookInput = ctx.ReadValue<Vector2>();
        };

        inputActions.Player.Look.canceled += ctx =>
        {
            lookInput = Vector2.zero;
        };
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void LateUpdate()
    {
        HandleLook();
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        yaw += mouseX;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 70f);

        transform.rotation = Quaternion.Euler(
            pitch,
            yaw,
            0f
        );
    }
}