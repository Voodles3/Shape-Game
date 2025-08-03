using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShapeMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Tooltip("The value to multiply the player's Y velocity by when the player releases jump. Lower values = more control over jump height.")]
    public float jumpDecayMultiplier = 0.5f;
    public float dashForce = 15f;
    public float dashCooldown = 1f;
    public float acceleration = 20f;

    [Header("Stamina Settings")]
    public float maxStamina;
    public float currentStamina;

    public float jumpCost;
    public float dashCost;
    public float attackCost;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    private Vector2 currentInputs;
    private bool isGrounded;
    private bool canDash = true;

    [Header("References")]
    public GameObject shapeSprite;
    public Slider staminaBar;

    private Animator animator;

    public void SetMoveInputs(Vector2 input) => currentInputs = input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = shapeSprite.GetComponent<Animator>();
        currentStamina = maxStamina;
    }

    void Update()
    {
        CheckGrounded();
    }

    void FixedUpdate()
    {
        MovePlayer();
        ChangeAnimationBools();
        
    }

    void ChangeAnimationBools()
    {
        if (isGrounded)
        {
            animator.SetBool("Jump", false);
        }
        if (canDash)
        {
            animator.SetBool("Dash", false);
        }
    }

    void MovePlayer()
    {
        float targetSpeed = currentInputs.x * moveSpeed;
        float appliedAccel = isGrounded ? acceleration : acceleration / 2f; // Halved acceleration if midair
        float newX = Mathf.Lerp(rb.linearVelocityX, targetSpeed, appliedAccel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocityY); // Setting velocity directly instead of adding force, for controlled acceleration and fighting game style movement
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            animator.SetBool("Jump", true);
        }
    }


    public void ReleaseJump()
    {
        if (rb.linearVelocityY > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * jumpDecayMultiplier);
            SubtractStamina(jumpCost);
        }
    }

    public void Dash()
    {
        if (canDash)
        {
            animator.SetBool("Dash",true);
            canDash = false;
            Vector2 dashDirection = new Vector2(currentInputs.x, 0).normalized;
            if (dashDirection == Vector2.zero)
                dashDirection = Vector2.right; // Dash right by default if player isn't moving - we could change this behavior

            rb.linearVelocity = new Vector2(rb.linearVelocityX + (dashDirection.x * dashForce), rb.linearVelocityY);

            SubtractStamina(dashCost);
            
            Invoke(nameof(ResetDash), dashCooldown);

        }
    }

    void ResetDash() => canDash = true;

    void CheckGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck transform not assigned.");
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void SubtractStamina(float val)
    {
        currentStamina -= val;
        UpdateStaminaBar();
    }
    public void UpdateStaminaBar()
    {
        staminaBar.value = currentStamina/maxStamina;
    }
}
