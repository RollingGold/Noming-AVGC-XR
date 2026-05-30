using UnityEngine;
using System.Collections;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 15f;
    [SerializeField] private float fallDelay = 1f;

    private bool canFall;

    private void OnEnable()
    {
        canFall = false;

        StartCoroutine(StartFalling());
    }

    private IEnumerator StartFalling()
    {
        yield return new WaitForSeconds(fallDelay);

        canFall = true;
    }

    private void Update()
    {
        if (!canFall)
            return;

        transform.position +=
            Vector3.down *
            fallSpeed *
            Time.deltaTime;

        if (transform.position.y < 0)
        {
            gameObject.SetActive(false);
        }
    }
}