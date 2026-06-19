using UnityEngine;

public class EnemyMiniMapDot : MonoBehaviour
{
    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float scaleMultiplierMin = 1f;
    [SerializeField] private float scaleMultiplierMax = 1.5f;

    [Header("Alpha")]
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;

    private Material materialInstance;
    private Vector3 originalScale;

    private void Awake()
    {
        materialInstance =
            GetComponent<Renderer>().material;

        originalScale =
            transform.localScale;
    }

    private void Update()
    {
        float t =
            (Mathf.Sin(Time.time * pulseSpeed) + 1f)
            * 0.5f;

        // Scale pulse
        float scaleMultiplier =
            Mathf.Lerp(
                scaleMultiplierMin,
                scaleMultiplierMax,
                t
            );

        transform.localScale =
            originalScale * scaleMultiplier;

        // Alpha pulse
        Color color =
            materialInstance.color;

        color.a =
            Mathf.Lerp(
                minAlpha,
                maxAlpha,
                t
            );

        materialInstance.color = color;
    }
}