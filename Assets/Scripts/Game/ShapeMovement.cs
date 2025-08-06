using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShapeMovement : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] private bool isPlayerControlled = true;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [Tooltip("The value to multiply the player's Y velocity by when the player releases jump. Lower values = more control over jump height.")]
    [SerializeField] private float jumpDecayMultiplier = 0.5f;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float acceleration = 20f;

    [Header("Stamina Settings (Player Only)")]
    [SerializeField] private Slider staminaBar;
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] private float staminaRegenRate;
    [SerializeField] private float staminaRegenDelay;
    [SerializeField] private float jumpCost;
    [SerializeField] private float dashCost;
    [SerializeField] private float attackCost;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Other References")]
    [SerializeField] private ParticleSystem jumpParticles;

    private float currentInputs;
    private float lastInputs;
    [HideInInspector] public bool canMove = true;

    private float baseMoveSpeed;
    private float staminaRegenTimer;
    private bool isGrounded;
    private bool canDash = true;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator animator;

    public float CurrentMoveSpeed => moveSpeed;
    public float CurrentStamina => currentStamina;
    public float CurrentInputs => currentInputs;
    public float LastInputs => lastInputs;
    public float AttackCost => attackCost;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        currentStamina = maxStamina;
        lastInputs = 1;
        baseMoveSpeed = moveSpeed;
    }

    void Update()
    {
        CheckGrounded();
        if (isPlayerControlled) RegenStamina();
    }

    void FixedUpdate()
    {
        if (currentInputs != 0)
        {
            lastInputs = currentInputs;
        }
        if (canMove)
        {
            Move();
        }
        UpdateAnimationBools();
    }

    void UpdateAnimationBools()
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

    void Move()
    {
        float targetSpeed = currentInputs * moveSpeed;
        float appliedAccel = isGrounded ? acceleration : acceleration / 2f; // Halved acceleration if midair
        float newX = Mathf.Lerp(rb.linearVelocityX, targetSpeed, appliedAccel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocityY); // Setting velocity directly instead of adding force, for controlled acceleration and fighting game style movement
    }

    public void Jump()
    {
        if (!isGrounded) return;
        if (isPlayerControlled && currentStamina < jumpCost) return;

        if (isPlayerControlled) SubtractStamina(jumpCost);

        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);

        animator.SetBool("Jump", true);
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
        if (!canDash) return;
        if (isPlayerControlled && currentStamina < dashCost) return;

        animator.SetBool("Dash", true);
        canDash = false;
        DashMove(dashForce);

        if (isPlayerControlled) SubtractStamina(dashCost);

        Invoke(nameof(ResetDash), dashCooldown);
    }

    public void SubtractStamina(float val)
    {
        if (!isPlayerControlled) return;

        staminaRegenTimer = 0f;
        currentStamina = Mathf.Max(currentStamina -= val, 0f);
        UpdateStaminaBar();
    }

    public void DashMove(float dashPower) // made this a seperate method because it will be called when attacking
    {
        Vector2 dashDirection = new Vector2(currentInputs, 0).normalized;
        if (dashDirection == Vector2.zero)
            dashDirection = new Vector2(lastInputs, 0).normalized; ; // Dash in direction player last moved

        rb.linearVelocity = new Vector2(rb.linearVelocityX + (dashDirection.x * dashPower), rb.linearVelocityY);
    }

    public void SetMoveInputs(float input) => currentInputs = input;

    private void ResetDash() => canDash = true;

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck transform not assigned.");
            return;
        }
        Bounds bounds = col.bounds;

        float checkHeight = 0.05f; // Thin strip under player
        float inset = 0.02f; // Inset from sides to avoid unwanted edge contact

        Vector2 areaTopLeft = new(bounds.min.x + inset, bounds.min.y - checkHeight);
        Vector2 areaBottomRight = new(bounds.max.x - inset, bounds.min.y);

        isGrounded = Physics2D.OverlapArea(areaTopLeft, areaBottomRight, groundLayer);
    }



    private void UpdateStaminaBar()
    {
        staminaBar.value = currentStamina / maxStamina;
    }

    private void RegenStamina()
    {
        if (currentStamina < maxStamina)
        {
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer >= staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
                UpdateStaminaBar();
            }
        }
    }

    public void SetTempMoveSpeed(float speed) => moveSpeed = speed;
    public void ResetMoveSpeed() => moveSpeed = baseMoveSpeed;

    private void OnEnable()
    {
        if (isPlayerControlled) PlayerManager.Instance.RegisterActivePlayer(transform);
    }

    private void OnDisable()
    {
        if (isPlayerControlled) PlayerManager.Instance.UnregisterActivePlayer();
    }

    // Uncomment if you want to visualize the ground check area (nah im good)
    // void OnDrawGizmosSelected()
    // {
    //     if (!Application.isPlaying) return; // Prevent errors if not running

    //     if (col == null)
    //         col = GetComponent<Collider2D>();
    //     if (col == null) return;

    //     Bounds bounds = col.bounds;

    //     float checkHeight = 0.05f;
    //     float inset = 0.02f;

    //     Vector2 topLeft = new Vector2(bounds.min.x + inset, bounds.min.y - checkHeight);
    //     Vector2 bottomRight = new Vector2(bounds.max.x - inset, bounds.min.y);
    //     Vector2 center = (topLeft + bottomRight) / 2f;
    //     Vector2 size = bottomRight - topLeft;

    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireCube(center, size);
    // }


}
