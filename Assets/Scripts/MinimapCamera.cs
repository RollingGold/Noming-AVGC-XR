using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform mainCamera;

    [SerializeField] private float height = 30f;

    private void LateUpdate()
    {
        transform.position =
            new Vector3(
                player.position.x,
                player.position.y + height,
                player.position.z
            );

        transform.rotation =
            Quaternion.Euler(
                90f,
                mainCamera.eulerAngles.y,
                0f
            );
    }
}