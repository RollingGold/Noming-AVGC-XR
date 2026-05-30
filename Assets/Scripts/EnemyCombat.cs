using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRange = 3;

    [Header("Colliders")]
    [SerializeField] private GameObject normalAttackCollider;
    [SerializeField] private GameObject meteor; 

    public float AttackRange => attackRange;

    private float attackCooldownLeft;

    private Animator animator;

    private Enemy enemy;

    private EnemyAI enemyAI;

    private Transform player;



    public enum AttackType
    {
        Normal,
        Meteor
    }

    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        enemy = GetComponent<Enemy>();

        enemyAI = GetComponent<EnemyAI>();

        player =
            GameObject.FindGameObjectWithTag("Player")
            .transform;

        attackCooldownLeft = 0f;
    }

    private void Update()
    {
        if (enemy.IsDead)
            return;

        HandleCooldown();



    }

    private void HandleCooldown()
    {
        attackCooldownLeft -= Time.deltaTime;

        if (attackCooldownLeft < 0f)
        {
            attackCooldownLeft = 0f;
        }
    }

    public bool CanAttack()
    {
        if (IsAttacking)
            return false;

        if (attackCooldownLeft > 0f)
            return false;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distance > AttackRange)
            return false;

        return true;
    }

    public void Attack(AttackType attackType)
    {
        switch(attackType)
        {
            case AttackType.Normal:
                if (!CanAttack())
                    return;

                normalAttackCollider.SetActive(true);

                IsAttacking = true;

                attackCooldownLeft = attackCooldown;

                animator.SetTrigger("Attack");
                break;
            case AttackType.Meteor:
                if (!CanAttack())
                    return;

                meteor.transform.position = new Vector3 (player.position.x, 13f, player.position.z );

                meteor.SetActive(true);

                IsAttacking = true;

                attackCooldownLeft = attackCooldown;
                animator.SetTrigger("Meteor");
                break;
        }
        
    }

    public void EndAttack()
    {
        normalAttackCollider.SetActive (false);
        IsAttacking = false;
    }

    // Called using animation event
    //public void DealDamage()
    //{
    //    float distance =
    //        Vector3.Distance(
    //            transform.position,
    //            player.position
    //        );

    //    if (distance > attackRange)
    //        return;

    //    PlayerHealth playerHealth =
    //        player.GetComponent<PlayerHealth>();

    //    if (playerHealth != null)
    //    {
    //        playerHealth.TakeDamage(damage);
    //    }
    //}
}