using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnMode
    {
        SpawnOnce,
        Infinite,
        Waves,
        Quest,
        Trigger,
        Time,
        Event
    }

    public enum SpawnShape
    {
        Point,
        Circle,
        Box,
        CustomPoints
    }


    // =========================================================
    // GENERAL
    // =========================================================

    [Header("General")]
    [SerializeField]
    private SpawnMode spawnMode = SpawnMode.SpawnOnce;

    [SerializeField]
    private SpawnShape spawnShape = SpawnShape.Circle;


    // =========================================================
    // ENEMY
    // =========================================================

    [Header("Enemy")]
    [SerializeField]
    private GameObject enemyPrefab;


    // =========================================================
    // SPAWN ONCE
    // =========================================================

    [Header("Spawn Once")]
    [SerializeField]
    private int spawnCount = 5;


    // =========================================================
    // INFINITE
    // =========================================================

    [Header("Infinite Spawn")]
    [SerializeField]
    private int maxAlive = 5;

    [SerializeField]
    private float respawnDelay = 5f;


    // =========================================================
    // SPAWN AREA
    // =========================================================

    [Header("Spawn Area")]
    [SerializeField]
    private float spawnRadius = 10f;


    // =========================================================
    // CLEANUP
    // =========================================================

    [Header("Cleanup")]
    [Tooltip(
        "Destroy all enemies created by this spawner " +
        "when the spawner is destroyed.")]
    [SerializeField]
    private bool destroySpawnedEnemiesOnDestroy = true;


    // =========================================================
    // DEBUG
    // =========================================================

    [Header("Debug")]
    [SerializeField]
    private bool drawGizmos = true;


    // =========================================================
    // RUNTIME
    // =========================================================

    private int currentAlive;

    private readonly List<GameObject> spawnedEnemies =
        new List<GameObject>();


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        switch (spawnMode)
        {
            case SpawnMode.SpawnOnce:
                SpawnOnce();
                break;

            case SpawnMode.Infinite:
                StartInfiniteSpawner();
                break;
        }
    }


    // =========================================================
    // SPAWN ONCE
    // =========================================================

    private void SpawnOnce()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
        }
    }


    // =========================================================
    // INFINITE
    // =========================================================

    private void StartInfiniteSpawner()
    {
        while (currentAlive < maxAlive)
        {
            SpawnEnemy();

            currentAlive++;
        }
    }


    public void EnemyDied(GameObject enemy)
    {
        if (enemy != null)
        {
            spawnedEnemies.Remove(enemy);
        }

        if (currentAlive > 0)
        {
            currentAlive--;
        }

        if (spawnMode == SpawnMode.Infinite)
        {
            StartCoroutine(
                RespawnRoutine());
        }
    }


    // Compatibility with your old Enemy script
    public void EnemyDied()
    {
        if (currentAlive > 0)
        {
            currentAlive--;
        }

        if (spawnMode == SpawnMode.Infinite)
        {
            StartCoroutine(
                RespawnRoutine());
        }
    }


    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(
            respawnDelay);

        // Spawner might have been destroyed
        if (this == null)
            yield break;

        if (currentAlive < maxAlive)
        {
            SpawnEnemy();

            currentAlive++;
        }
    }


    // =========================================================
    // SPAWN ENEMY
    // =========================================================

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning(
                "EnemySpawner has no enemy prefab.",
                this);

            return;
        }

        Vector3 spawnPosition =
            GetSpawnPosition();

        GameObject enemy =
            Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity);

        // Keep track of the enemy.
        spawnedEnemies.Add(enemy);

        Enemy enemyScript =
            enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.SetSpawner(this);
        }
    }


    // =========================================================
    // SPAWN POSITION
    // =========================================================

    private Vector3 GetSpawnPosition()
    {
        switch (spawnShape)
        {
            case SpawnShape.Point:

                return transform.position;


            case SpawnShape.Circle:

                return GetRandomCirclePoint();


            case SpawnShape.Box:

                return GetRandomBoxPoint();


            default:

                return transform.position;
        }
    }


    private Vector3 GetRandomCirclePoint()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 random =
                Random.insideUnitCircle *
                spawnRadius;

            Vector3 point =
                transform.position +
                new Vector3(
                    random.x,
                    0f,
                    random.y);


            if (NavMesh.SamplePosition(
                point,
                out NavMeshHit hit,
                2f,
                NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return transform.position;
    }


    private Vector3 GetRandomBoxPoint()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 point =
                transform.position +
                new Vector3(
                    Random.Range(
                        -spawnRadius,
                        spawnRadius),

                    0f,

                    Random.Range(
                        -spawnRadius,
                        spawnRadius));


            if (NavMesh.SamplePosition(
                point,
                out NavMeshHit hit,
                2f,
                NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return transform.position;
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (!destroySpawnedEnemiesOnDestroy)
            return;

        DestroySpawnedEnemies();
    }


    private void DestroySpawnedEnemies()
    {
        // Make a copy because the original
        // list is modified as objects are destroyed.
        for (int i = spawnedEnemies.Count - 1;
             i >= 0;
             i--)
        {
            GameObject enemy =
                spawnedEnemies[i];

            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();

        currentAlive = 0;
    }


    // =========================================================
    // MANUAL CLEANUP
    // =========================================================

    public void KillAllEnemies()
    {
        DestroySpawnedEnemies();
    }


    public void KillOneEnemy()
    {
        for (int i = 0;
             i < spawnedEnemies.Count;
             i++)
        {
            GameObject enemy =
                spawnedEnemies[i];

            if (enemy != null)
            {
                Destroy(enemy);

                spawnedEnemies.RemoveAt(i);

                if (currentAlive > 0)
                    currentAlive--;

                return;
            }
        }
    }


    // =========================================================
    // RANDOM POINT
    // =========================================================

    public Vector3 GetRandomPointInSpawner()
    {
        return GetSpawnPosition();
    }


    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.green;

        switch (spawnShape)
        {
            case SpawnShape.Point:

                Gizmos.DrawSphere(
                    transform.position,
                    0.3f);

                break;


            case SpawnShape.Circle:

                Gizmos.DrawWireSphere(
                    transform.position,
                    spawnRadius);

                break;


            case SpawnShape.Box:

                Gizmos.DrawWireCube(
                    transform.position,
                    Vector3.one *
                    spawnRadius *
                    2f);

                break;
        }
    }
}