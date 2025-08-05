using UnityEngine;

public class CircleAttack : ShapeAttack
{

    public PhysicsMaterial2D bounceMaterial;
    PhysicsMaterial2D ogMaterial;
    public float bounceSpeed;


    public override void Attack()
    {
        base.Attack();
    }

    public override void SpecialAttack()
    {
        if (!CanSpecialAttack()) return;
        base.SpecialAttack();
        ogMaterial = rb.sharedMaterial;
        movement.canMove = false; 
        rb.sharedMaterial = bounceMaterial;
        //rb.gravityScale = 0f;
        damage = specialAttackDamage;
        specialAttackEnabled = true;
        StartBouncing();
    }

    public override void StopSpecialAttack()
    {
        base.StopSpecialAttack();
        movement.canMove = true;
        rb.sharedMaterial = ogMaterial;
        rb.gravityScale = normalGravity;
        rb.linearVelocity = new Vector2(0f, 0f);
        damage = ogDamage;
        specialAttackEnabled = false;
    }

    void StartBouncing()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = dir * bounceSpeed;
    }
}
