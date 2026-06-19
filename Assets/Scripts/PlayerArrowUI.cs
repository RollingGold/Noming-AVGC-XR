using UnityEngine;

public class PlayerArrowUI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform mainCamera;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float angle =
            player.eulerAngles.y -
            mainCamera.eulerAngles.y;

        rectTransform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                -angle
            );
    }
}