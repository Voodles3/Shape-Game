using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShapeMovement : MonoBehaviour
{
    [Header("Control Settings")]
    public bool isPlayerControlled = true;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    private float baseMoveSpeed;

    [Tooltip("The value to multiply the player's Y velocity by when the player releases jump. Lower values = more control over jump height.")]
    public float jumpDecayMultiplier = 0.5f;
    public float dashForce = 15f;
    public float dashCooldown = 1f;
    public float acceleration = 20f;

    [Header("Stamina Settings (Player Only)")]
    public float maxStamina;
    public float currentStamina;
    public float staminaRegenRate;
    public float staminaRegenDelay;

    public float jumpCost;
    public float dashCost;
    public float attackCost;

    private float staminaRegenTimer;

    [Header("Mana Settings")]

    public float maxMana;
    public float currentMana;
    public float manaRegenRate;
    public float manaRegenDelay;

    public float manaRegenTimer;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Collider2D col;

    [HideInInspector] public Vector2 currentInputs;
    [HideInInspector] public Vector2 lastInputs;
    private bool isGrounded;
    private bool canDash = true;
    [HideInInspector] public bool canMove = true;

    [Header("References")]
    public GameObject shapeSprite;
    public Slider staminaBar;
    public Slider manaBar;
    public ParticleSystem jumpParticles;

    private Animator animator;

    public void SetMoveInputs(Vector2 input) => currentInputs = input;
    public float CurrentMoveSpeed() => moveSpeed;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = shapeSprite.GetComponent<Animator>();
        currentStamina = maxStamina;
        currentMana = 0f;
        lastInputs.x = 1;
        baseMoveSpeed = moveSpeed;

        UpdateManaBar();
    }

    void Update()
    {
        CheckGrounded();
        if (isPlayerControlled) RegenStamina();
        if (isPlayerControlled) RegenMana();
    }

    void FixedUpdate()
    {
        if (currentInputs.x != 0)
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
        float targetSpeed = currentInputs.x * moveSpeed;
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
        Bounds bounds = col.bounds;

        float checkHeight = 0.05f; // Thin strip under player
        float inset = 0.02f; // Inset from sides to avoid unwanted edge contact

        Vector2 areaTopLeft = new(bounds.min.x + inset, bounds.min.y - checkHeight);
        Vector2 areaBottomRight = new(bounds.max.x - inset, bounds.min.y);

        isGrounded = Physics2D.OverlapArea(areaTopLeft, areaBottomRight, groundLayer);
    }

    public void SubtractStamina(float val)
    {
        if (!isPlayerControlled) return;

        staminaRegenTimer = 0f;
        currentStamina = Mathf.Max(currentStamina -= val, 0f);
        UpdateStaminaBar();
    }

    public void ResetMana()
    {
        if (!isPlayerControlled) return;

        manaRegenTimer = 0f;
        currentMana = 0f;
        UpdateManaBar();
    }

    public void UpdateStaminaBar()
    {
        staminaBar.value = currentStamina / maxStamina;
    }

    public void UpdateManaBar()
    {
        if (!isPlayerControlled) return;
        manaBar.value = currentMana / maxMana;
    }

    void RegenStamina()
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

    void RegenMana()
    {
        if (currentMana < maxMana)
        {
            manaRegenTimer += Time.deltaTime;
            if (manaRegenTimer >= manaRegenDelay)
            {
                currentMana += manaRegenRate * Time.deltaTime;
                currentMana = Mathf.Min(currentMana, maxMana);
                UpdateManaBar();
            }
        }
    }

    public void SetTempMoveSpeed(float speed) => moveSpeed = speed;
    public void ResetMoveSpeed() => moveSpeed = baseMoveSpeed;

    void OnEnable()
    {
        if (isPlayerControlled) PlayerManager.Instance.RegisterActivePlayer(transform);
    }

    void OnDisable()
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
