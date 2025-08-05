using UnityEngine;

// This class is inheritable by our shape-specific attack scripts, and makes them require an Attack() method
public abstract class ShapeAttack : MonoBehaviour
{
    public GameObject shapeSprite;
    public Collider2D attackHitbox;

    public bool canAttack = true;
    public bool isAttacking = false;
    public bool isSpecialAttacking = false;
    public float attackCooldown;
    public float attackDuration;
    public int damage;
    public float attackDashForce;

    public float specialAttackDuration;
    public int specialAttackDamage;

    private Animator animator;
    private Rigidbody2D rb;
    public ShapeMovement movement;
    public Health health;
    private float normalGravity;

    public void Start()
    {
        animator = shapeSprite.GetComponent<Animator>();
        movement = GetComponent<ShapeMovement>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        normalGravity = rb.gravityScale;

        attackHitbox.enabled = false;
    }

    public virtual void Attack()
    {
        if (!canAttack || isSpecialAttacking) return;
        if (movement.currentStamina < movement.attackCost) return;

        canAttack = false;
        isAttacking = true;
        animator.SetBool("attack", true);

        Vector2 attackDirection = movement.currentInputs.x != 0
            ? new Vector2(movement.currentInputs.x, 0)
            : new Vector2(movement.lastInputs.x, 0);

        rb.gravityScale = 0f;
        animator.SetFloat("attackDirection", attackDirection.x);
        movement.DashMove(attackDashForce);

        movement.SubtractStamina(movement.attackCost);

        attackHitbox.enabled = true;

        Invoke(nameof(StopAttack), attackDuration);
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    public virtual void SpecialAttack()
    {
        if (isAttacking) return;
        if (movement.currentMana < movement.maxMana) return;
        isSpecialAttacking = true;
        movement.ResetMana();

        Invoke(nameof(StopSpecialAttack), specialAttackDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking) return;

        if (other.TryGetComponent(out Health health) && other.gameObject != gameObject)
        {
            health.TakeDamage(damage);
            attackHitbox.enabled = false;
        }
    }

    void StopAttack()
    {
        animator.SetBool("attack", false);
        isAttacking = false;
        rb.gravityScale = normalGravity;

        attackHitbox.enabled = false;
    }

    void ResetAttack() => canAttack = true;


    public virtual void StopSpecialAttack()
    {
        isSpecialAttacking = false;
    }
}
