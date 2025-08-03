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
    public float staminaRegenRate;
    public float staminaDelay;

    public float jumpCost;
    public float dashCost;
    public float attackCost;

    private float staminaRegenTimer;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    public Vector2 currentInputs;
    public Vector2 lastInputs;
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
        lastInputs.x = 1;
    }

    void Update()
    {
        CheckGrounded();
        RegenStamina();
    }

    void FixedUpdate()
    {
        MovePlayer();
        ChangeAnimationBools();
        if (currentInputs.x != 0)
        {
            lastInputs = currentInputs;
        }
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
        if (isGrounded && currentStamina >= jumpCost)
        {
            SubtractStamina(jumpCost);
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            animator.SetBool("Jump", true);
        }
    }


    public void ReleaseJump()
    {
        if (rb.linearVelocityY > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * jumpDecayMultiplier);
        }
    }

    public void Dash()
    {
        if (canDash && currentStamina >= dashCost)
        {
            animator.SetBool("Dash", true);
            canDash = false;
            DashMove(dashForce);

            SubtractStamina(dashCost);

            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    public void DashMove(float dashPower) // made this a seperate method because it will be called when attacking
    {
        Vector2 dashDirection = new Vector2(currentInputs.x, 0).normalized;
        if (dashDirection == Vector2.zero)
            dashDirection = new Vector2(lastInputs.x, 0).normalized; ; // Dash in direction player last moved

        rb.linearVelocity = new Vector2(rb.linearVelocityX + (dashDirection.x * dashPower), rb.linearVelocityY);
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
        staminaRegenTimer = 0f;
        currentStamina = Mathf.Max(currentStamina, 0f);
        UpdateStaminaBar();
    }

    public void UpdateStaminaBar()
    {
        staminaBar.value = currentStamina / maxStamina;
    }

    void RegenStamina()
    {
        if (currentStamina < maxStamina)
        {
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer >= staminaDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
                UpdateStaminaBar();
            }
        }
    }
}
