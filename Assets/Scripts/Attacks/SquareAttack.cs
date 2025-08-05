using System.Collections;
using UnityEngine;

public class SquareAttack : ShapeAttack // Inherits our abstract class
{

    public float moveSpeedDuringSpecialAttack;
    public override void Attack()
    {
        base.Attack();
    }

    public override void SpecialAttack()
    {
        if (!CanSpecialAttack()) return;
        base.SpecialAttack();
        movement.moveSpeed = moveSpeedDuringSpecialAttack;
        damage = specialAttackDamage;
        health.isSquareSpecialAttacking = true;
    }

    public override void StopSpecialAttack()
    {
        base.StopSpecialAttack();
        movement.moveSpeed = movement.originalMoveSpeed;
        damage = ogDamage;
        health.isSquareSpecialAttacking = false;
    }
}
