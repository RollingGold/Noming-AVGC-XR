using UnityEngine;

public class ItemVisual : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 60f;

    [Header("Floating")]
    [SerializeField] private float floatHeight = 0.25f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition =
            transform.position;
    }

    private void Update()
    {
        // Rotation
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.unscaledDeltaTime,
            Space.World
        );

        // Floating
        Vector3 position =
            startPosition;

        position.y +=
            Mathf.Sin(
                Time.unscaledTime * floatSpeed
            ) * floatHeight;

        transform.position =
            position;
    }
}