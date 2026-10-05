using System.Collections;
using UnityEngine;

public class Skill1WaveSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform player;

    [Header("Spawn")]
    [SerializeField] private int spawnCount = 3;
    [SerializeField] private float spawnDelay = 0.3f;

    [Header("Rotation")]
    [Tooltip("Additional Y rotation applied to the player's facing direction.")]
    [SerializeField] private float yRotationOffset = 0f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveDistance = 10f;

    [Header("Scale")]
    [SerializeField] private float startXScale = 1f;
    [SerializeField] private float startZScale = 1f;

    [SerializeField] private float endXScale = 3f;
    [SerializeField] private float endZScale = 3f;


    // =========================================================
    // USE THIS FROM OTHER SCRIPTS
    // =========================================================

    public void ActivateSkill()
    {
        if (prefab == null)
        {
            Debug.LogError(
                "Skill1WaveSpawner: Prefab is not assigned.",
                this);

            return;
        }

        if (player == null)
        {
            Debug.LogError(
                "Skill1WaveSpawner: Player is not assigned.",
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
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnPrefab();

            if (i < spawnCount - 1)
            {
                yield return new WaitForSeconds(
                    spawnDelay);
            }
        }
    }


    // =========================================================
    // SPAWN
    // =========================================================

    private void SpawnPrefab()
    {
        // Movement direction = player's forward
        Vector3 direction = player.forward;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();


        // Object rotation = player rotation + offset
        Quaternion spawnRotation =
            Quaternion.Euler(
                0f,
                player.eulerAngles.y + yRotationOffset,
                0f
            );


        GameObject spawned =
            Instantiate(
                prefab,
                transform.position,
                spawnRotation
            );


        StartCoroutine(
            MoveAndGrow(
                spawned,
                direction
            )
        );
    }


    // =========================================================
    // MOVE + GROW
    // =========================================================

    private IEnumerator MoveAndGrow(
        GameObject obj,
        Vector3 direction)
    {
        if (obj == null)
            yield break;


        Vector3 startPosition =
            obj.transform.position;


        float distanceTravelled = 0f;


        // -----------------------------------------------------
        // START SCALE
        // -----------------------------------------------------

        Vector3 startScale =
            obj.transform.localScale;


        startScale.x =
            startXScale;

        startScale.z =
            startZScale;


        // -----------------------------------------------------
        // END SCALE
        // -----------------------------------------------------

        Vector3 endScale =
            startScale;


        endScale.x =
            endXScale;

        endScale.z =
            endZScale;


        obj.transform.localScale =
            startScale;


        // -----------------------------------------------------
        // MOVE
        // -----------------------------------------------------

        while (
            obj != null &&
            distanceTravelled < moveDistance)
        {
            float movement =
                moveSpeed *
                Time.deltaTime;


            distanceTravelled +=
                movement;


            if (distanceTravelled >
                moveDistance)
            {
                distanceTravelled =
                    moveDistance;
            }


            // -------------------------------------------------
            // Move in player's direction + Y offset
            // -------------------------------------------------

            obj.transform.position =
                startPosition +
                direction *
                distanceTravelled;


            // -------------------------------------------------
            // Grow
            // -------------------------------------------------

            float progress =
                distanceTravelled /
                moveDistance;


            obj.transform.localScale =
                Vector3.Lerp(
                    startScale,
                    endScale,
                    progress
                );


            yield return null;
        }


        // -----------------------------------------------------
        // DESTROY
        // -----------------------------------------------------

        if (obj != null)
        {
            Destroy(obj);
        }
    }
}