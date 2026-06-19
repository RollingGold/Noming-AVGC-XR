using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Parameter")]
    [SerializeField] private float attackCooldown = 3f;


    private InputSystem_Actions inputActions;

    private Player player;

    private Animator animator;

    private PlayerMovement playerMovement;

    private GameObject weaponCollider;

    private bool attackPressed;

    public float AttackCooldown => attackCooldown;
    public float attackCooldownLeft { get; private set; }

    public bool isAttacking {  get; private set; }

    private void Awake()
    {
        player = GetComponent<Player>();

        attackCooldownLeft = 0f;

        playerMovement = GetComponent<PlayerMovement>();

        animator = GetComponent<Animator>();

        weaponCollider = GameObject.FindGameObjectWithTag("Weapon Collider");

        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {

        inputActions.Enable();

        inputActions.Player.Attack.performed += ctx =>
        {
            attackPressed = true;
        };
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (player.IsDead)
            return;

        HandleAttack();

    }

    private void HandleAttack()
    {

        attackCooldownLeft -= Time.deltaTime;

        if (!attackPressed)
            return;


        if (attackCooldownLeft >= 0) 
            return;

        if (isAttacking)
            return;

        if (!playerMovement.isGrounded)
            return;

        attackPressed = false;

        isAttacking = true;




        animator.SetTrigger("Attack");
    }
    


    //Animation Events

    public void EndAttack()
    {
        isAttacking = false;

        attackCooldownLeft = attackCooldown;
    }

    public void EnableWeaponCollider()
    {

        weaponCollider.SetActive(true);
    }

    public void DisableWeaponCollider()
    {
        weaponCollider.SetActive(false);

    }
}