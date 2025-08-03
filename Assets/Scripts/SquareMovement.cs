using UnityEngine;
using UnityEngine.InputSystem;

public class SquareMovement : MonoBehaviour
{
    InputActions inputActions;
    InputAction jumpAction;
    InputAction dashAction;
    Rigidbody2D rb;

    Vector2 inputs;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float dashForce = 15f;
    public float dashCooldown = 1f;
    public float acceleration = 20f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    bool isGrounded;
    bool canDash = true;
    bool justJumped = false;

    void Awake()
    {
        inputActions = InputManager.Instance.InputActions;

        jumpAction = inputActions.Player.Jump;
        dashAction = inputActions.Player.Dash;
    }

    void OnEnable()
    {
        jumpAction.performed += OnJump;
        dashAction.performed += OnDash;
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        jumpAction.performed -= OnJump;
        dashAction.performed -= OnDash;
        inputActions.Player.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ReceiveInput();
        CheckGrounded();
    }

    void FixedUpdate()
    {
        MovePlayer();
        justJumped = false;
    }

    void ReceiveInput()
    {
        inputs = inputActions.Player.Move.ReadValue<Vector2>();
    }

    void MovePlayer()
    {
        float targetSpeed = inputs.x * moveSpeed;
        float newAcceleration = isGrounded ? acceleration : acceleration / 2f;
        float newX = Mathf.Lerp(rb.linearVelocityX, targetSpeed, newAcceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocityY);
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            justJumped = true;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    void OnDash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            canDash = false;
            Vector2 dashDirection = new Vector2(inputs.x, 0).normalized;
            if (dashDirection == Vector2.zero)
                dashDirection = Vector2.right;

            rb.linearVelocity = new Vector2(dashDirection.x * dashForce, rb.linearVelocityY);
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    void ResetDash()
    {
        canDash = true;
    }

    void CheckGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck transform not assigned.");
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
