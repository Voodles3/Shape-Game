using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShapeMovement : MonoBehaviour
{
    [HideInInspector] public bool canMove = true; // This is the ONE field I've found so far that makes sense being public. 
                                                  // It needs to be read here, is def movement related so it should live here, and needs to be set by other scripts.

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
    [SerializeField] private LayerMask groundLayer;

    [Header("Other References")]
    [SerializeField] private ParticleSystem jumpParticles;

    private float currentInputs;
    private float lastInputs;
    private float baseMoveSpeed;
    private float staminaRegenTimer;
    private bool isGrounded;
    private bool canDash = true;
    private bool isPlayer;

    private static readonly int JumpAnimBool = Animator.StringToHash("Jump");
    private static readonly int DashAnimBool = Animator.StringToHash("Dash");

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator animator;

    // None of these should be set outside of this class, so I made them get-only properties. This is the best way to do it instead of making the field public.
    // This one-liner notation is basically shorthand for get; private set;
    public float CurrentMoveSpeed => moveSpeed;
    public float CurrentStamina => currentStamina;
    public float CurrentInputs => currentInputs;
    public float LastInputs => lastInputs;
    public float AttackCost => attackCost;

    public bool IsPlayer => isPlayer;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();

        currentStamina = maxStamina;
        lastInputs = 1;
        baseMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        if (currentInputs != 0) lastInputs = currentInputs;
        if (isPlayer) RegenStamina();

        CheckGrounded();
        ResetAnimationBools();
    }

    private void FixedUpdate()
    {
        if (canMove) Move();
    }

    public void Jump()
    {
        if (!isGrounded) return;
        if (isPlayer && currentStamina < jumpCost) return;

        if (isPlayer) SubtractStamina(jumpCost);

        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);

        animator.SetBool(JumpAnimBool, true);
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
        if (isPlayer && currentStamina < dashCost) return;

        animator.SetBool(DashAnimBool, true);
        canDash = false;
        DashMove(dashForce);

        if (isPlayer) SubtractStamina(dashCost);

        Invoke(nameof(ResetDash), dashCooldown);
    }

    public void DashMove(float dashPower)
    {
        Vector2 dashDirection = new Vector2(currentInputs, 0).normalized;
        if (dashDirection == Vector2.zero)
            dashDirection = new Vector2(lastInputs, 0).normalized; // Dash in direction player last moved

        rb.linearVelocity = new Vector2(rb.linearVelocityX + (dashDirection.x * dashPower), rb.linearVelocityY);
    }

    public void SubtractStamina(float val)
    {
        if (!isPlayer) return;

        staminaRegenTimer = 0f;
        currentStamina = Mathf.Max(currentStamina - val, 0f);
        UpdateStaminaBar();
    }

    public void SetMoveInputs(float input) => currentInputs = input;

    public void SetTempMoveSpeed(float speed) => moveSpeed = speed;
    public void ResetMoveSpeed() => moveSpeed = baseMoveSpeed;

    private void Move()
    {
        float targetSpeed = currentInputs * moveSpeed;
        float appliedAccel = isGrounded ? acceleration : acceleration / 2f; // Halved acceleration if midair
        float newX = Mathf.Lerp(rb.linearVelocityX, targetSpeed, appliedAccel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocityY); // Setting velocity directly instead of adding force, for controlled acceleration and fighting game style movement
    }

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

    private void ResetAnimationBools()
    {
        if (isGrounded)
        {
            animator.SetBool(JumpAnimBool, false);
        }
        if (canDash)
        {
            animator.SetBool(DashAnimBool, false);
        }
    }

    private void OnEnable()
    {
        isPlayer = CompareTag("Player");
        if (isPlayer) PlayerManager.Instance.RegisterActivePlayer(transform);
    }

    private void OnDisable()
    {
        if (isPlayer) PlayerManager.Instance.UnregisterActivePlayer();
    }

    private void ResetDash() => canDash = true;
    private void UpdateStaminaBar() => staminaBar.value = currentStamina / maxStamina;

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
