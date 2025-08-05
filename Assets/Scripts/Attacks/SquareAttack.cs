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
        base.SpecialAttack();
        movement.moveSpeed = moveSpeedDuringSpecialAttack;
        health.isSquareSpecialAttacking = true;
    }

    public override void StopSpecialAttack()
    {
        base.StopSpecialAttack();
        movement.moveSpeed = movement.originalMoveSpeed;
        health.isSquareSpecialAttacking = false;
    }
}
