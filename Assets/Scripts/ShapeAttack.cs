using UnityEngine;

// This class is inheritable by our shape-specific attack scripts, and makes them require an Attack() method
public abstract class ShapeAttack : MonoBehaviour
{
    public GameObject shapeSprite;
    Animator animator;
    Rigidbody2D rb;
    ShapeMovement movement;
    public bool canAttack = true;
    public bool isAttacking = false;
    public float attackCooldown;
    public float attackDuration;
    public int damage;
    public float attackDashForce;
    float normalGravity;

    public void Start()
    {
        animator = shapeSprite.GetComponent<Animator>();
        movement = GetComponent<ShapeMovement>();
        rb = GetComponent<Rigidbody2D>();
        normalGravity = rb.gravityScale;
    }



    public virtual void Attack()
    {
        if (canAttack && movement.currentStamina >= movement.attackCost)
        {
            canAttack = false;
            isAttacking = false;
            animator.SetBool("attack", true);
            Vector2 attackDirection = new Vector2(movement.currentInputs.x, 0).normalized;
            if (movement.currentInputs.x == 0)
                attackDirection = new Vector2(movement.lastInputs.x, 0).normalized;
            Debug.Log(attackDirection.x);
            rb.gravityScale = 0f;
            animator.SetFloat("attackDirection", attackDirection.x);
            movement.DashMove(attackDashForce);
            
            movement.SubtractStamina(movement.attackCost);
            Invoke(nameof(StopAttack), attackDuration);
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
        }
    }

    void StopAttack()
    {
        animator.SetBool("attack", false);
        isAttacking = false;
        rb.gravityScale = normalGravity;
    }

    void ResetAttack() => canAttack = true;
}
