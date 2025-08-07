using System.Collections;
using UnityEngine;

public class SquareAttack : ShapeAttack
{
    [Tooltip("Multiplier for square's attack damage while special attacking.")]
    [SerializeField] private float specialAttackDamageMultiplier = 1.5f;
    [Tooltip("Multiplier for how much damage player is recieving during ult.")]
    [SerializeField] private float specialAttackDamageResistance = 0.5f;

    public override void SpecialAttack()
    {
        if (!CanSpecialAttack()) return;
        base.SpecialAttack();

        Health.AddDamageMultiplier(specialAttackDamageResistance);
    }

    public override void StopSpecialAttack()
    {
        base.StopSpecialAttack();
        Health.RemoveDamageMultiplier(specialAttackDamageMultiplier);
    }
}
