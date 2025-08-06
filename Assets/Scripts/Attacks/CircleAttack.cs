using UnityEngine;

public class CircleAttack : ShapeAttack
{
    [SerializeField] private float bounceSpeed;
    [SerializeField] private PhysicsMaterial2D bounceMaterial;

    private PhysicsMaterial2D baseMaterial;

    public override void SpecialAttack()
    {
        if (!CanSpecialAttack()) return;
        base.SpecialAttack();
        baseMaterial = Rb.sharedMaterial;
        ToggleMovement(false);
        Rb.sharedMaterial = bounceMaterial;
        StartBouncing();
    }

    public override void StopSpecialAttack()
    {
        base.StopSpecialAttack();
        ToggleMovement(true);
        Rb.sharedMaterial = baseMaterial;
        Rb.linearVelocity = Vector2.zero;
    }

    void StartBouncing()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        Rb.linearVelocity = dir * bounceSpeed;
    }
}
