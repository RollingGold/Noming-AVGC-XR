using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 9f;

    private void Update()
    {
        transform.position +=
            Vector3.down *
            fallSpeed *
            Time.deltaTime;
    }
}
