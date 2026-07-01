using System.Collections;
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

    [Header("General")]
    [SerializeField] private SpawnMode spawnMode = SpawnMode.SpawnOnce;
    [SerializeField] private SpawnShape spawnShape = SpawnShape.Circle;

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Once")]
    [SerializeField] private int spawnCount = 5;

    [Header("Infinite Spawn")]
    [SerializeField] private int maxAlive = 5;
    [SerializeField] private float respawnDelay = 5f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnRadius = 10f;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private int currentAlive;

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

    #region Spawn Once

    private void SpawnOnce()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
        }
    }

    #endregion

    #region Infinite

    private void StartInfiniteSpawner()
    {
        while (currentAlive < maxAlive)
        {
            SpawnEnemy();

            currentAlive++;
        }
    }

    public void EnemyDied()
    {
        currentAlive--;

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (currentAlive < maxAlive)
        {
            SpawnEnemy();

            currentAlive++;
        }
    }

    #endregion

    #region Spawn Enemy

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetSpawnPosition();

        GameObject enemy =
            Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity);

        Enemy enemyScript =
            enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.SetSpawner(this);
        }
    }

    #endregion

    #region Spawn Position

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
                Random.insideUnitCircle * spawnRadius;

            Vector3 point =
                transform.position +
                new Vector3(random.x, 0f, random.y);

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
                    Random.Range(-spawnRadius, spawnRadius),
                    0f,
                    Random.Range(-spawnRadius, spawnRadius));

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

    #endregion

    #region Debug

    public void KillAllEnemies()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void KillOneEnemy()
    {
        GameObject enemy =
            GameObject.FindGameObjectWithTag("Enemy");

        if (enemy != null)
        {
            Destroy(enemy);
        }
    }


    #endregion

    public Vector3 GetRandomPointInSpawner()
    {
        return GetSpawnPosition();
    }

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
                    Vector3.one * spawnRadius * 2f);
                break;
        }
    }
}