using UnityEngine;

// This class is inheritable by our shape-specific attack scripts, and makes them require an Attack() method
public abstract class ShapeAttack : MonoBehaviour
{
    public GameObject shapeSprite;
    Animator animator;
    ShapeMovement movement;
    public bool canAttack = true;
    public bool isAttacking = false;
    public float attackCooldown;
    public float attackDuration;

    public void Start()
    {
        animator = shapeSprite.GetComponent<Animator>();
        movement = GetComponent<ShapeMovement>();
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
            animator.SetFloat("attackDirection", attackDirection.x);
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
                //do dmagae
            }
        }
    }

    void StopAttack()
    {
        animator.SetBool("attack", false);
        isAttacking = false;

    }

    void ResetAttack() => canAttack = true;
}
