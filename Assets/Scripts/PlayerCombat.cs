using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Basic Attack")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float attackPower = 100f;
    [SerializeField] private BoxCollider attackCollider;

    [Header("Skill 1")]
    [SerializeField] private float skill1Cooldown = 5f;
    [SerializeField] private float skill1Power = 130f;
    [SerializeField] private Skill1WaveSpawner skill1WaveSpawner;

    [Header("Skill 2")]
    [SerializeField] private float skill2Cooldown = 8f;
    [SerializeField] private float skill2Power = 150f;
    [SerializeField] private Skill2SphereSpawner skill2SphereSpawner;

    [Header("Skill 3")]
    [SerializeField] private float skill3Cooldown = 13f;
    [SerializeField] private float skill3Power = 300f;
    [SerializeField] private Skill3SphereSpawner skill3SphereSpawner;


    private InputSystem_Actions inputActions;

    private Player player;
    private Animator animator;
    private PlayerMovement playerMovement;

    private bool attackPressed;
    private bool skill1Pressed;
    private bool skill2Pressed;
    private bool skill3Pressed;

    public float AttackCooldown => attackCooldown;
    public float AttackPower => attackPower;

    public float Skill1Cooldown => skill1Cooldown;
    public float Skill1Power => skill1Power;

    public float Skill2Cooldown => skill2Cooldown;
    public float Skill2Power => skill2Power;

    public float Skill3Cooldown => skill3Cooldown;
    public float Skill3Power => skill3Power;

    public float attackCooldownLeft { get; private set; }
    public float skill1CooldownLeft { get; private set; }
    public float skill2CooldownLeft { get; private set; }
    public float skill3CooldownLeft { get; private set; }

    public bool isAttacking { get; private set; }


    private void Awake()
    {
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        inputActions = new InputSystem_Actions();

        attackCollider.enabled = false;

        attackCooldownLeft = 0f;
        skill1CooldownLeft = 0f;
        skill2CooldownLeft = 0f;
        skill3CooldownLeft = 0f;
    }


    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Attack.performed += OnAttackPressed;
        inputActions.Player.Skill1.performed += OnSkill1Pressed;
        inputActions.Player.Skill2.performed += OnSkill2Pressed;
        inputActions.Player.Skill3.performed += OnSkill3Pressed;
    }


    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttackPressed;
        inputActions.Player.Skill1.performed -= OnSkill1Pressed;
        inputActions.Player.Skill2.performed -= OnSkill2Pressed;
        inputActions.Player.Skill3.performed -= OnSkill3Pressed;

        inputActions.Disable();
    }


    private void OnAttackPressed(InputAction.CallbackContext context)
    {
        attackPressed = true;
    }


    private void OnSkill1Pressed(InputAction.CallbackContext context)
    {
        skill1Pressed = true;
    }


    private void OnSkill2Pressed(InputAction.CallbackContext context)
    {
        skill2Pressed = true;
    }


    private void OnSkill3Pressed(InputAction.CallbackContext context)
    {
        skill3Pressed = true;
    }


    private void Update()
    {
        if (player.IsDead)
            return;

        UpdateCooldowns();

        if (GameInputManager.InputLocked)
            return;

        HandleAttack();
        HandleSkill1();
        HandleSkill2();
        HandleSkill3();
    }


    private void UpdateCooldowns()
    {
        if (attackCooldownLeft > 0f)
            attackCooldownLeft -= Time.deltaTime;

        if (skill1CooldownLeft > 0f)
            skill1CooldownLeft -= Time.deltaTime;

        if (skill2CooldownLeft > 0f)
            skill2CooldownLeft -= Time.deltaTime;

        if (skill3CooldownLeft > 0f)
            skill3CooldownLeft -= Time.deltaTime;
    }


    // =========================================================
    // BASIC ATTACK
    // =========================================================

    private void HandleAttack()
    {
        if (!attackPressed)
        {
            attackPressed = false;
            return;
        }

        if (isAttacking)
        {
            attackPressed = false;
            return;
        }

        if (attackCooldownLeft > 0f)
        {
            attackPressed = false;
            return;
        }

        if (!playerMovement.isGrounded)
        {
            attackPressed = false;
            return;
        }

       

        StartAttack("Attack");

        attackCooldownLeft = attackCooldown;
    }


    // =========================================================
    // SKILL 1
    // =========================================================

    private void HandleSkill1()
    {
        if (!skill1Pressed)
        {
            skill1Pressed = false;
            return;
        }

        if (isAttacking)
        {
            skill1Pressed = false;
            return;
        }

        if (skill1CooldownLeft > 0f)
        {
            skill1Pressed = false;
            return;
        }

        if (!playerMovement.isGrounded)
        {
            skill1Pressed = false;
            return;
        }

        skill1Pressed = false;

        StartAttack("Skill1");

        skill1CooldownLeft = skill1Cooldown;
    }


    // =========================================================
    // SKILL 2
    // =========================================================

    private void HandleSkill2()
    {
        if (!skill2Pressed)
        {
            skill2Pressed = false;
            return;
        }

        if (isAttacking)
        {
            skill2Pressed = false;
            return;
        }

        if (skill2CooldownLeft > 0f)
        {
            skill2Pressed = false;
            return;
        }

        if (!playerMovement.isGrounded)
        {
            skill2Pressed = false;
            return;
        }

        skill2Pressed = false;

        StartAttack("Skill2");

        skill2CooldownLeft = skill2Cooldown;
    }


    // =========================================================
    // SKILL 3
    // =========================================================

    private void HandleSkill3()
    {
        if (!skill3Pressed)
        {
            skill3Pressed = false;
            return;
        }

        if (isAttacking)
        {
            skill3Pressed = false;
            return;
        }

        if (skill3CooldownLeft > 0f)
        {
            skill3Pressed = false;
            return;
        }

        if (!playerMovement.isGrounded)
        {
            skill3Pressed = false;
            return;
        }

        skill3Pressed = false;

        StartAttack("Skill3");

        skill3CooldownLeft = skill3Cooldown;
    }


    // =========================================================
    // START
    // =========================================================

    private void StartAttack(string animationTrigger)
    {
        isAttacking = true;

        animator.SetTrigger(animationTrigger);
    }


    // =========================================================
    // ANIMATION EVENTS
    // =========================================================

    // Basic Attack

    public void EnableWeaponCollider()
    {
        attackCollider.enabled = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        attackCollider.enabled = false;
    }


    // Skill 1

    public void ActivateSkill1()
    {


        skill1WaveSpawner.ActivateSkill();
        
    }

    public void EndSkill1()
    {
        isAttacking = false;
    }


    // Skill 2

    public void ActivateSkill2()
    {
        skill2SphereSpawner.ActivateSkill();
    }

    public void EndSkill2()
    {
        isAttacking = false;
    }


    // Skill 3

    public void ActivateSkill3()
    {
        skill3SphereSpawner.ActivateSkill();
    }

    public void EndSkill3()
    {
        isAttacking = false;
    }
}