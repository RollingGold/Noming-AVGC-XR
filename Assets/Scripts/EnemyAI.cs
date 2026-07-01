using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static EnemyCombat;

public class EnemyAI : MonoBehaviour
{
    public enum PatrolMode
    {
        None,
        PatrolPoints,
        RandomRadius,
        SpawnerRadius
    }

    [Header("Detection")]
    [SerializeField] private float chaseRange = 10f;

    [Header("Patrol")]
    [SerializeField] private PatrolMode patrolMode;

    [Header("Waypoint Patrol")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("Random Patrol")]
    [SerializeField] private float patrolRadius = 10f;

    [Header("Common Patrol")]
    [SerializeField] private float patrolWaitTime = 2f;

    private Transform player;
    private NavMeshAgent agent;
    private Enemy enemy;
    private EnemyCombat enemyCombat;
    private EnemyStateMachine stateMachine;

    private bool waiting;
    private bool movingForward = true;
    private bool hasPatrolDestination;

    private int currentPatrolIndex;

    private Vector3 homePosition;
    private Vector3 currentDestination;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemyCombat = GetComponent<EnemyCombat>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<EnemyStateMachine>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        homePosition = transform.position;
    }

    private void Update()
    {
        if (enemy.IsDead)
        {
            agent.isStopped = true;
            return;
        }

        if (enemyCombat.IsAttacking)
        {
            agent.isStopped = true;
            return;
        }
            

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= enemyCombat.AttackRange)
        {
            agent.isStopped = true;

            enemyCombat.Attack(Random.Range(0, 2) == 0 ?
                AttackType.Normal :
                AttackType.Meteor);

            return;
        }

        if (distance <= chaseRange)
        {
            hasPatrolDestination = false;
            ChasePlayer();
            return;
        }

        if (patrolMode == PatrolMode.None)
        {
            Idle();
            return;
        }

        Patrol();
    }

    private void ChasePlayer()
    {
        stateMachine.ChangeState(EnemyStateMachine.EnemyState.Moving);
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void Patrol()
    {
        if (waiting)
        {
            Idle();
            return;
        }

        stateMachine.ChangeState(EnemyStateMachine.EnemyState.Moving);
        agent.isStopped = false;

        if (!hasPatrolDestination)
        {
            currentDestination = GetNextPatrolPoint();
            agent.SetDestination(currentDestination);
            hasPatrolDestination = true;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitForNextPatrol());
        }
    }

    private IEnumerator WaitForNextPatrol()
    {
        waiting = true;
        agent.isStopped = true;
        stateMachine.ChangeState(EnemyStateMachine.EnemyState.Idle);

        yield return new WaitForSeconds(patrolWaitTime);

        hasPatrolDestination = false;
        waiting = false;
    }

    private Vector3 GetNextPatrolPoint()
    {
        switch (patrolMode)
        {
            case PatrolMode.PatrolPoints:
                return GetNextWaypoint();

            case PatrolMode.RandomRadius:
                return GetRandomPatrolPoint();

            case PatrolMode.SpawnerRadius:
                if (enemy.Spawner != null)
                    return enemy.Spawner.GetRandomPointInSpawner();
                return GetRandomPatrolPoint();

            default:
                return transform.position;
        }
    }

    private Vector3 GetNextWaypoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return transform.position;

        Vector3 point = patrolPoints[currentPatrolIndex].position;

        if (movingForward)
        {
            currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPoints.Length)
            {
                currentPatrolIndex = patrolPoints.Length - 2;
                movingForward = false;
            }
        }
        else
        {
            currentPatrolIndex--;

            if (currentPatrolIndex < 0)
            {
                currentPatrolIndex = 1;
                movingForward = true;
            }
        }

        currentPatrolIndex = Mathf.Clamp(currentPatrolIndex, 0, patrolPoints.Length - 1);

        return point;
    }

    private Vector3 GetRandomPatrolPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 random = Random.insideUnitCircle * patrolRadius;

            Vector3 point = homePosition +
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

        return homePosition;
    }

    private void Idle()
    {
        stateMachine.ChangeState(EnemyStateMachine.EnemyState.Idle);
        agent.isStopped = true;
    }

    public void Die()
    {
        agent.isStopped = true;
    }
}
