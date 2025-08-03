using UnityEngine;

public class SquareAttack : ShapeAttack // Inherits our abstract class
{
    public override void Attack()
    {
        Debug.Log("Square Attack");
    }
}
