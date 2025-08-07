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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy != null)
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag(enemy.gameObject.tag))
                StopSpecialAttack();
    }

    public override void SpecialAttack()
    {
        if (!CanSpecialAttack()) return;
        base.SpecialAttack();
        if (base.IsPlayer())
        {
            if (enemy == null)
                enemy = GameObject.FindWithTag("Enemy");
        }
        else
        {
            if (enemy == null)
                enemy = GameObject.FindWithTag("Player");
        }
        ToggleMovement(false);
        Health.AddDamageMultiplier(specialAttackDamageResistance);
        Rb.freezeRotation = false;
        Rb.gravityScale = 0f;
        isAiming = true;
        Invoke(nameof(StopAiming), aimingTime);
    }

    public override void StopSpecialAttack()
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
        transform.Translate(shootDirection * shootSpeed * Time.deltaTime, Space.World);
    }

    private void StopAiming()
    {
        isAiming = false;
        shootDirection = (targetPos - transform.position).normalized;
    }
}
