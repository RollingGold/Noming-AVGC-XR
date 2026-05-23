using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeedMultiplier = 1.3f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -30f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField]private float fallingCooldown = 1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;

    private InputSystem_Actions inputActions;

    private PlayerStateMachine stateMachine;

    private PlayerCombat playerCombat;

    private Vector2 moveInput;

    private Vector3 velocity;

    private float currentSpeedMultiplier = 1;

    private float currentSpeed = 0;

    private float fallingCooldownLeft;

    private bool jumpPressed;

    private bool sprintPressed;

    private bool canMove;

    public bool isGrounded {  get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        stateMachine = GetComponent<PlayerStateMachine>();

        inputActions = new InputSystem_Actions();

        playerCombat = GetComponent<PlayerCombat>();

        fallingCooldownLeft = fallingCooldown;
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
        };

        inputActions.Player.Move.canceled += ctx =>
        {
            moveInput = Vector2.zero;
        };

        inputActions.Player.Jump.performed += ctx =>
        {
            jumpPressed = true;
        };

        inputActions.Player.Sprint.performed += ctx =>
        {
            sprintPressed = true;
            currentSpeedMultiplier += sprintSpeedMultiplier;
        };

        inputActions.Player.Sprint.canceled += ctx =>
        {
            sprintPressed = false;
            currentSpeedMultiplier -= sprintSpeedMultiplier;
        };
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        CheckCanMove();

        CheckGround();

        CalculateCurrentSpeed();

        HandleMovement();

        HandleJump();

        HandleGravity();

        HandleStateTransitions();
    }

    private void HandleMovement()
    {

        if (!canMove)
            return;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * moveInput.y +
            cameraRight * moveInput.x;

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        

        controller.Move(
            moveDirection.normalized *
            currentSpeed *
            Time.deltaTime
        );
    }

    private void CalculateCurrentSpeed()
    {
        currentSpeed = moveSpeed;

        currentSpeed = currentSpeed * currentSpeedMultiplier;

    }

    private void HandleJump()
    {
        if(!canMove)
            return;

        if (playerCombat.isAttacking)
            return;


        if (!jumpPressed)
            return;

        if (!isGrounded)
        {
            jumpPressed = false;
            return;
        }

        velocity.y = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );

        jumpPressed = false;
    }

    private void HandleGravity()
    {
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (velocity.y < 0f)
        {
            velocity.y += gravity *
                fallMultiplier *
                Time.deltaTime;
        }
        else
        {
            velocity.y += gravity *
                Time.deltaTime;
        }

        controller.Move(
            velocity * Time.deltaTime
        );
    }

    public void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundLayer
        );
    }

    private void HandleStateTransitions()
    {
        fallingCooldownLeft -= Time.deltaTime;

        if( isGrounded )
        {
            fallingCooldownLeft = fallingCooldown;
        }

        bool isMoving = moveInput.magnitude > 0.1f;

        bool isFalling = velocity.y < -0.1f && !isGrounded;

        bool isJumping = velocity.y > 0.1f;

        bool isSprinting =
            sprintPressed &&
            isMoving;

        if (isFalling && fallingCooldownLeft <= 0)
        {
            stateMachine.ChangeState(
                PlayerStateMachine.PlayerState.Fall
            );

            return;
        }

        if (isJumping)
        {
            stateMachine.ChangeState(
                PlayerStateMachine.PlayerState.Jump
            );

            return;
        }

        if (isSprinting)
        {
            stateMachine.ChangeState(
                PlayerStateMachine.PlayerState.Sprinting
            );

            return;
        }

        if (isMoving)
        {
            stateMachine.ChangeState(
                PlayerStateMachine.PlayerState.Walking
            );

            return;
        }

        stateMachine.ChangeState(
            PlayerStateMachine.PlayerState.Idle
        );
    }

    private void CheckCanMove()
    {
        if(playerCombat.isAttacking)
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundDistance
        );
    }
}