using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 1000;

    [Header("Combat")]
    [SerializeField] private int attackDamage = 25;

    private int currentHealth;

    public bool IsDead { get; private set; }

    public float HealthPercent =>
        (float)currentHealth / maxHealth;

    public int AttackDamage => attackDamage;

    private Animator animator;

    private void Awake()
    {

        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        Debug.Log("Player HP:" +  currentHealth);
    }

    private void Die()
    {
        IsDead = true;

        animator.SetTrigger("Dead");

        Debug.Log("Player Died");
    }
}