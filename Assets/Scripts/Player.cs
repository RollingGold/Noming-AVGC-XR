using UnityEngine;

public class Player : MonoBehaviour
{
    // =========================================================
    // HEALTH
    // =========================================================

    [Header("Health")]
    [SerializeField] private int maxHealth = 1000;


    // =========================================================
    // COMBAT
    // =========================================================

    [Header("Combat")]
    [SerializeField] private int attackDamage = 25;


    // =========================================================
    // STATE
    // =========================================================

    public int currentHealth
    {
        get;
        private set;
    }


    public int MaxHealth =>
        maxHealth;


    public bool IsDead
    {
        get;
        private set;
    }


    public float HealthPercent =>
        maxHealth > 0
            ? (float)currentHealth / maxHealth
            : 0f;


    public int AttackDamage =>
        attackDamage;


    // =========================================================
    // REFERENCES
    // =========================================================

    private Animator animator;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        animator =
            GetComponent<Animator>();


        // Start with full health.
        currentHealth =
            maxHealth;


        IsDead = false;
    }


    // =========================================================
    // TAKE DAMAGE
    // =========================================================

    public void TakeDamage(int damage)
    {
        if (IsDead)
            return;


        if (damage <= 0)
            return;


        currentHealth -= damage;


        // Prevent negative health.
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            Die();
        }


        Debug.Log(
            "Player HP: " +
            currentHealth +
            " / " +
            maxHealth);
    }


    // =========================================================
    // HEAL
    // =========================================================

    public void Heal(int amount)
    {
        if (IsDead)
            return;


        if (amount <= 0)
            return;


        currentHealth += amount;


        if (currentHealth > maxHealth)
        {
            currentHealth =
                maxHealth;
        }


        Debug.Log(
            "Player Healed: " +
            currentHealth +
            " / " +
            maxHealth);
    }


    // =========================================================
    // SET HEALTH
    // =========================================================

    public void SetHealth(int health)
    {
        if (IsDead)
            return;


        currentHealth =
            Mathf.Clamp(
                health,
                0,
                maxHealth);


        if (currentHealth <= 0)
        {
            Die();
        }
    }


    // =========================================================
    // DEATH
    // =========================================================

    private void Die()
    {
        if (IsDead)
            return;


        IsDead = true;

        currentHealth = 0;


        if (animator != null)
        {
            animator.SetTrigger(
                "Dead");
        }


        Debug.Log(
            "Player Died");
    }
}