using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Parameter")]
    [SerializeField] private float attackCooldown = 3;


    private InputSystem_Actions inputActions;

    private Player player;

    private Animator animator;

    private PlayerMovement playerMovement;

    private GameObject weaponCollider;

    private bool attackPressed;

    private float attackCooldownLeft;

    public bool isAttacking {  get; private set; }

    private void Awake()
    {
        player = GetComponent<Player>();

        attackCooldownLeft = attackCooldown;

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
        if (!attackPressed)
            return;

        attackCooldownLeft += Time.deltaTime;

        if (attackCooldownLeft < attackCooldown) return;

        if (isAttacking)
            return;

        if (!playerMovement.isGrounded)
            return;

        attackPressed = false;

        isAttacking = true;

        attackCooldownLeft = 0f;



        animator.SetTrigger("Attack");
    }
    


    //Animation Events

    public void EndAttack()
    {
        isAttacking = false;
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