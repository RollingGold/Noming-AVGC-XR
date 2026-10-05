using System.Collections;
using UnityEngine;

public class Skill2SphereSpawner : MonoBehaviour
{
    [Header("Sphere")]
    [SerializeField] private GameObject spherePrefab;

    [Header("Spawn")]
    [SerializeField] private int sphereCount = 10;
    [SerializeField] private float spawnDelay = 0.2f;

    [Header("Range")]
    [SerializeField] private float spawnRange = 5f;

    [Header("Height")]
    [SerializeField] private float spawnHeight = 2f;

    [Header("Gizmo")]
    [SerializeField] private bool showGizmo = true;

    [SerializeField]
    private Color gizmoColor =
        new Color(1f, 0f, 0f, 0.25f);


    // =========================================================
    // ACTIVATE SKILL
    // =========================================================

    public void ActivateSkill()
    {
        if (spherePrefab == null)
        {
            Debug.LogError(
                "Skill2SphereSpawner: Sphere Prefab is not assigned.",
                this);

            return;
        }

        StartCoroutine(
            SpawnRoutine());
    }


    // =========================================================
    // SPAWN ROUTINE
    // =========================================================

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < sphereCount; i++)
        {
            SpawnSphere();

            if (i < sphereCount - 1)
            {
                yield return new WaitForSeconds(
                    spawnDelay);
            }
        }
    }


    // =========================================================
    // SPAWN SPHERE
    // =========================================================

    private void SpawnSphere()
    {
        Vector2 random =
            Random.insideUnitCircle *
            spawnRange;


        Vector3 spawnPosition =
            transform.position +
            new Vector3(
                random.x,
                spawnHeight,
                random.y
            );


        Instantiate(
            spherePrefab,
            spawnPosition,
            Quaternion.identity
        );
    }


    // =========================================================
    // GIZMO
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (!showGizmo)
            return;


        Gizmos.color =
            gizmoColor;


        // Filled range
        Gizmos.DrawSphere(
            transform.position,
            spawnRange
        );


        // Wire range
        Gizmos.color =
            Color.red;


        Gizmos.DrawWireSphere(
            transform.position,
            spawnRange
        );
    }
}