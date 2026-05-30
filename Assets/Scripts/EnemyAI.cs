using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static EnemyCombat;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float chaseRange = 10f;


    private float attackCooldownLeft;

    private Transform player;

    private NavMeshAgent agent;

    private Enemy enemy;

    private EnemyCombat enemyCombat;

    private EnemyStateMachine stateMachine;

    private void Awake()
    {

        enemy = GetComponent<Enemy>();

        enemyCombat = GetComponent<EnemyCombat>();

        agent = GetComponent<NavMeshAgent>();

        stateMachine = GetComponent<EnemyStateMachine>();

        player =
            GameObject.FindGameObjectWithTag("Player")
            .transform;

    }

    private void Update()
    {
        if (enemy.IsDead)
            return;
        if(enemyCombat.IsAttacking)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        


        if (distance <= enemyCombat.AttackRange)
        {
            agent.isStopped = true;

            int randomAttack = Random.Range(0, 2);

            switch(randomAttack)
            {
                case 0: 
                    enemyCombat.Attack(AttackType.Normal);
                    break;
                case 1:
                    enemyCombat.Attack(AttackType.Meteor);
                    break;
            }
            
            
            
        }
        else if (distance <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

  

    private void ChasePlayer()
    {
        stateMachine.ChangeState(EnemyStateMachine.EnemyState.Sprinting);

        agent.isStopped = false;

        agent.SetDestination(player.position);
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