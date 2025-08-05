using UnityEngine;

// This class is inheritable by our shape-specific attack scripts, and makes them require an Attack() method
public abstract class ShapeAttack : MonoBehaviour
{
    public GameObject shapeSprite;
    public Collider2D attackHitbox;

    public bool canAttack = true;
    public bool isAttacking = false;
    public float attackCooldown;
    public float attackDuration;
    public int damage;
    public float attackDashForce;

    private Animator animator;
    private Rigidbody2D rb;
    private ShapeMovement movement;
    private float normalGravity;

    public void Start()
    {
        animator = shapeSprite.GetComponent<Animator>();
        movement = GetComponent<ShapeMovement>();
        rb = GetComponent<Rigidbody2D>();
        normalGravity = rb.gravityScale;

        attackHitbox.enabled = false;
    }

    public virtual void Attack()
    {
        if (!canAttack) return;
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
}
