using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Parameter")]
    [SerializeField] private float attackCooldown = 3;


    private InputSystem_Actions inputActions;

    private Animator animator;

    private PlayerMovement playerMovement;

    private bool attackPressed;

    private float attackCooldownLeft;

    public bool isAttacking {  get; private set; }

    private void Awake()
    {

        attackCooldownLeft = attackCooldown;

        playerMovement = GetComponent<PlayerMovement>();

        animator = GetComponent<Animator>();

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
        
        EndAttack();

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

        

        attackCooldownLeft = 0f;



        animator.SetTrigger("Attack");
    }

    public void EndAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Attack"))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
    }

}