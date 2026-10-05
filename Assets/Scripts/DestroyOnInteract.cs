using System.Collections;
using UnityEngine;

public class DestroyOnInteract : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField]
    private string interactionText = "OK";

    [Header("Animation")]
    [SerializeField]
    private float moveUpDistance = 2f;

    [SerializeField]
    private float duration = 0.75f;

    private bool interacting;

    public string InteractionText => interactionText;

    public void Interact()
    {
        if (interacting)
            return;

        interacting = true;

        StartCoroutine(
            MoveUpAndDisappear());
    }

    private IEnumerator MoveUpAndDisappear()
    {
        Vector3 startPosition =
            transform.position;

        Vector3 endPosition =
            startPosition +
            Vector3.up * moveUpDistance;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration);

            // Smooth movement
            t = Mathf.SmoothStep(
                0f,
                1f,
                t);

            transform.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    t);

            yield return null;
        }

        transform.position = endPosition;

        gameObject.SetActive(false);
    }
}