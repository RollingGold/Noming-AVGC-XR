using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Boss Phases")]
    [SerializeField] private int[] phaseHealth;
    [SerializeField] private bool carryOverDamageBetweenPhases;

    [Header("Stats")]
    [SerializeField] private float attackDamage = 13f; 

    [Header("Death")]
    [SerializeField] private float despawnDelay = 10f;

    public float AttackDamage => attackDamage;

    private int currentPhase;

    private int currentHealth;

    private Animator animator;

    private EnemyAI enemyAI;

    public bool IsDead { get; private set; }

    public int CurrentPhase
    {
        get
        {
            return currentPhase + 1;
        }
    }

    public float HealthPercent
    {
        get
        {
            return
                (float)currentHealth /
                phaseHealth[currentPhase];
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        enemyAI = GetComponent<EnemyAI>();

        currentPhase = 0;

        currentHealth = phaseHealth[currentPhase];
    }

    public void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        currentHealth -= damage;

        Debug.Log(
            "Enemy took damage. HP: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            NextPhase();
        }
    }

    private void NextPhase()
    {
        int remainingDamage =
            Mathf.Abs(currentHealth);

        currentPhase++;

        if (currentPhase >= phaseHealth.Length)
        {
            Die();
            return;
        }

        currentHealth =
            phaseHealth[currentPhase];

        if (carryOverDamageBetweenPhases)
        {
            currentHealth -= remainingDamage;
            Debug.Log("Enemy took Remaining Dmg. HP:" + currentHealth);
        }

        EnterPhase(currentPhase);
    }

    private void EnterPhase(int phase)
    {
        Debug.Log(
            "Entered Phase " +
            (phase + 1)
        );

        switch (phase)
        {
            case 1:

                // Phase 2 logic here
                // Example:
                // increase speed
                // unlock meteor attacks

                break;

            case 2:

                // Final phase logic here

                break;
        }
    }

    private void Die()
    {
        IsDead = true;

        animator.SetTrigger("Die");

        Debug.Log("Boss defeated");

        Destroy(gameObject, despawnDelay);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetCurrentPhaseMaxHealth()
    {
        return phaseHealth[currentPhase];
    }
}