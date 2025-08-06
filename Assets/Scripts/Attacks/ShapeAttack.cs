using UnityEngine;
using UnityEngine.UI;

public abstract class ShapeAttack : MonoBehaviour
{
    // ZAZ LET'S FOLLOW THIS FIELD ORDER:
    // 1. Public fields (let's avoid these though, use a property instead if needed)
    // 2. Serialized private fields
    // 3. Protected fields
    // 4. Private fields
    // 5. Private internal references
    // 6. Public getter methods or properties (at the end, even if they're public)
    // AS I'VE DONE BELOW:

    // Serialized private fields
    [Header("Attack")]
    [SerializeField] private Collider2D attackHitbox;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackDashForce;

    [Header("Special Attack")]
    [SerializeField] private int specialAttackDamage;
    [SerializeField] private float specialAttackDuration;
    [SerializeField] private int specialAttackMoveSpeed;

    [Header("Mana Settings")]
    [SerializeField] private Slider manaBar;
    [SerializeField] private float maxMana;
    [SerializeField] private float currentMana;
    [SerializeField] private float manaRegenRate;
    [SerializeField] private float manaRegenDelay;

    // Protected fields (accessible by derived classes)
    protected Rigidbody2D Rb { get; private set; }
    protected Health Health { get; private set; }

    // Private fields
    private bool canAttack = true;
    private bool isAttacking = false;
    private bool isSpecialAttacking = false;
    private float baseDamage;
    private float normalGravity;
    private float manaRegenTimer;

    // I changed these string refs to static readonly StringToHash ints so that we won't have string reference issues
    private static readonly int AttackBoolAnim = Animator.StringToHash("attack");
    private static readonly int AttackDirectionAnim = Animator.StringToHash("attackDirection");

    // Internal references
    private Animator animator;
    private ShapeMovement movement;

    // Getter methods and properties
    public bool IsAttacking() => isAttacking;
    public bool IsSpecialAttacking() => isSpecialAttacking;

    // SIMILARLY, LET'S FOLLOW THIS METHOD ORDER:
    // 1. Unity Methods (Start, Update, FixedUpdate, etc.)
    // 2. Public methods
    // 3. Protected methods
    // 4. Private methods
    // Put one-liners at the end of their respective section
    // AS I'VE DONE BELOW:

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        movement = GetComponent<ShapeMovement>();
        Rb = GetComponent<Rigidbody2D>();
        Health = GetComponent<Health>();

        currentMana = 0f;
        normalGravity = Rb.gravityScale;
        baseDamage = damage;

        ToggleHitbox(false);
        UpdateManaBar();
    }

    protected virtual void Update()
    {
        RegenMana();
    }

    public virtual void Attack()
    {
        if (!canAttack
        || isAttacking
        || movement.CurrentStamina < movement.AttackCost) return;

        canAttack = false;
        isAttacking = true;
        animator.SetBool(AttackBoolAnim, true);
        Rb.gravityScale = 0f;
        ToggleHitbox(true);

        Vector2 attackDirection = movement.currentInputs.x != 0
            ? new Vector2(movement.currentInputs.x, 0)
            : new Vector2(movement.lastInputs.x, 0);

        animator.SetFloat(AttackDirectionAnim, attackDirection.x);

        movement.DashMove(attackDashForce);
        movement.SubtractStamina(movement.AttackCost);

        Invoke(nameof(StopAttack), attackDuration);
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    public virtual void SpecialAttack()
    {
        isSpecialAttacking = true;
        ToggleHitbox(true);
        SetTempDamage(specialAttackDamage);

        ResetMana();
        movement.SetTempMoveSpeed(specialAttackMoveSpeed);

        Invoke(nameof(StopSpecialAttack), specialAttackDuration);
    }

    public virtual void StopSpecialAttack()
    {
        isSpecialAttacking = false;
        ToggleHitbox(false);
        ResetDamage();
        movement.ResetMoveSpeed();
    }

    public bool CanSpecialAttack() => !isAttacking && currentMana >= maxMana;

    public void ToggleMovement(bool canMove) => movement.canMove = canMove;

    public void SetTempDamage(int value) => damage = value;
    public void ResetDamage() => damage = baseDamage;

    private void StopAttack()
    {
        animator.SetBool(AttackBoolAnim, false);
        isAttacking = false;
        Rb.gravityScale = normalGravity;

        ToggleHitbox(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking && !isSpecialAttacking) return;

        if (other.TryGetComponent(out Health health) && other.gameObject != gameObject)
        {
            health.TakeDamage(damage);
            attackHitbox.enabled = false;
        }
    }

    #region Mana
    private void ResetMana()
    {

        manaRegenTimer = 0f;
        currentMana = 0f;
        UpdateManaBar();
    }

    private void UpdateManaBar()
    {
        manaBar.value = currentMana / maxMana;
    }

    private void RegenMana()
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
    #endregion

    private void ResetAttack() => canAttack = true;

    private void ToggleHitbox(bool enable) => attackHitbox.enabled = enable;
}
