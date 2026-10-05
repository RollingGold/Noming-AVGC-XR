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
    [SerializeField] private float fallingCooldown = 1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private InputSystem_Actions inputActions;

    private Player player;
    private PlayerStateMachine stateMachine;
    private PlayerCombat playerCombat;

    private Vector2 moveInput;
    private Vector3 velocity;

    private float currentSpeedMultiplier = 1f;
    private float currentSpeed;

    private float fallingCooldownLeft;

    private bool jumpPressed;
    private bool sprintPressed;

    public bool isGrounded
    {
        get;
        private set;
    }


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        player =
            GetComponent<Player>();

        controller =
            GetComponent<CharacterController>();

        stateMachine =
            GetComponent<PlayerStateMachine>();

        playerCombat =
            GetComponent<PlayerCombat>();

        inputActions =
            new InputSystem_Actions();

        fallingCooldownLeft =
            fallingCooldown;
    }


    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Player.Jump.performed += OnJump;

        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;
    }


    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Jump.performed -= OnJump;

        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;

        inputActions.Disable();
    }


    // =========================================================
    // INPUT
    // =========================================================

    private void OnMove(
        InputAction.CallbackContext context)
    {
        moveInput =
            context.ReadValue<Vector2>();
    }


    private void OnMoveCanceled(
        InputAction.CallbackContext context)
    {
        moveInput =
            Vector2.zero;
    }


    private void OnJump(
        InputAction.CallbackContext context)
    {
        jumpPressed = true;
    }


    private void OnSprint(
        InputAction.CallbackContext context)
    {
        sprintPressed = true;
    }


    private void OnSprintCanceled(
        InputAction.CallbackContext context)
    {
        sprintPressed = false;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (player.IsDead)
            return;


        if (GameInputManager.InputLocked)
            return;


        CheckGround();

        CalculateCurrentSpeed();

        HandleJump();

        HandleGravity();

        HandleMovement();

        HandleStateTransitions();
    }


    // =========================================================
    // MOVEMENT
    // =========================================================

    private void HandleMovement()
    {
        // Do not allow movement while attacking.
        if (playerCombat != null &&
            playerCombat.isAttacking)
        {
            return;
        }


        Vector3 cameraForward =
            cameraTransform.forward;

        Vector3 cameraRight =
            cameraTransform.right;


        // Remove vertical camera rotation.
        cameraForward.y = 0f;
        cameraRight.y = 0f;


        cameraForward.Normalize();
        cameraRight.Normalize();


        Vector3 moveDirection =
            cameraForward * moveInput.y +
            cameraRight * moveInput.x;


        // Prevent diagonal movement from being faster.
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }


        // -----------------------------------------------------
        // ROTATION
        // -----------------------------------------------------

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    moveDirection,
                    Vector3.up
                );


            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed *
                    Time.deltaTime
                );
        }


        // -----------------------------------------------------
        // HORIZONTAL MOVEMENT
        // -----------------------------------------------------

        Vector3 horizontalVelocity =
            moveDirection *
            currentSpeed;


        // -----------------------------------------------------
        // COMBINE HORIZONTAL + VERTICAL
        // -----------------------------------------------------

        Vector3 finalMovement =
            horizontalVelocity;

        finalMovement.y =
            velocity.y;


        controller.Move(
            finalMovement *
            Time.deltaTime
        );
    }


    // =========================================================
    // SPEED
    // =========================================================

    private void CalculateCurrentSpeed()
    {
        currentSpeed =
            moveSpeed;


        if (sprintPressed &&
            moveInput.magnitude > 0.1f)
        {
            currentSpeed *=
                sprintSpeedMultiplier;
        }
    }


    // =========================================================
    // JUMP
    // =========================================================

    private void HandleJump()
    {
        if (playerCombat != null &&
            playerCombat.isAttacking)
        {
            jumpPressed = false;
            return;
        }


        if (!jumpPressed)
            return;


        if (!isGrounded)
        {
            jumpPressed = false;
            return;
        }


        velocity.y =
            Mathf.Sqrt(
                jumpHeight *
                -2f *
                gravity
            );


        jumpPressed = false;
    }


    // =========================================================
    // GRAVITY
    // =========================================================

    private void HandleGravity()
    {
        if (isGrounded &&
            velocity.y < 0f)
        {
            velocity.y = -2f;
        }


        if (velocity.y < 0f)
        {
            velocity.y +=
                gravity *
                fallMultiplier *
                Time.deltaTime;
        }
        else
        {
            velocity.y +=
                gravity *
                Time.deltaTime;
        }
    }


    // =========================================================
    // GROUND
    // =========================================================

    public void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded =
                controller.isGrounded;

            return;
        }


        isGrounded =
            Physics.CheckSphere(
                groundCheck.position,
                groundDistance,
                groundLayer
            );


        // CharacterController fallback.
        if (controller.isGrounded)
        {
            isGrounded = true;
        }
    }


    // =========================================================
    // STATE MACHINE
    // =========================================================

    private void HandleStateTransitions()
    {
        fallingCooldownLeft -=
            Time.deltaTime;


        if (isGrounded)
        {
            fallingCooldownLeft =
                fallingCooldown;
        }


        bool isMoving =
            moveInput.magnitude > 0.1f;


        bool isFalling =
            velocity.y < -0.1f &&
            !isGrounded;


        bool isJumping =
            velocity.y > 0.1f;


        bool isSprinting =
            sprintPressed &&
            isMoving;


        if (isFalling &&
            fallingCooldownLeft <= 0f)
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


    // =========================================================
    // DEBUG
    // =========================================================

    private void OnDrawGizmos()
    {
        if (groundCheck == null)
            return;


        Gizmos.color =
            Color.green;


        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundDistance
        );
    }
}