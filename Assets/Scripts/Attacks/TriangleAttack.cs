using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TriangleAttack : ShapeAttack
{
    [SerializeField] private float aimingTime;
    [SerializeField] private float shootSpeed;
    private bool isAiming;
    private GameObject enemy;
    private Vector3 targetPos;
    private Vector3 shootDirection;
    [Tooltip("Multiplier for how much damage player is recieving during ult.")]
    [SerializeField] private float specialAttackDamageResistance = 0.5f;

    protected override void Update()
    {
        base.Update();
        if (IsSpecialAttacking())
        {
            if (isAiming)
            {
                Aim();
            }
            else
            {
                Shoot();
            }
        }
    }

    public override void SpecialAttack()
    {
        if (!CanSpecialAttack()) return;
        base.SpecialAttack();
        if (IsPlayer() && enemy == null)
        {
            enemy = GameObject.FindWithTag("Enemy");
        }
        else
        {
            enemy = GameObject.FindWithTag("Player");
        }
        ToggleMovement(false);
        Health.AddDamageMultiplier(specialAttackDamageResistance);
        Rb.freezeRotation = false;
        Rb.gravityScale = 0f;
        isAiming = true;
        Invoke(nameof(StopAiming), aimingTime);
    }

    protected override void StopSpecialAttack()
    {
        base.StopSpecialAttack();
        ToggleMovement(true);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Rb.freezeRotation = true;
        Rb.gravityScale = base.NormalGravity();
        Health.RemoveDamageMultiplier(specialAttackDamageResistance);
    }

    private void Aim()
    {
        if (enemy != null)
        {
            Vector2 direction = enemy.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            targetPos = enemy.transform.position;
        }
    }

    private void Shoot()
    {
        transform.Translate(shootSpeed * Time.deltaTime * shootDirection, Space.World); // Bro is a NErd
    }

    private void StopAiming()
    {
        isAiming = false;
        shootDirection = (targetPos - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnAttackContact(other);
        if (enemy != null && !isAiming)
            StopSpecialAttack();
    }
}
