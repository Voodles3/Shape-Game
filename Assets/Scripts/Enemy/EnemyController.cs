using UnityEngine;

[RequireComponent(typeof(ShapeMovement), typeof(ShapeAttack))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Idle, Aggressive, Retreating }

    [Header("Ranges")]
    public float attackRange = 1.8f;
    public float dashRange = 4f;
    public float jumpDistance = 2f;

    [Header("Timers")]
    public float attackCooldown = 1.2f;
    public float jumpCooldown = 1.5f;
    public float dashCooldown = 2f;
    public float decisionInterval = 0.2f;

    [Header("Probabilities")]
    [Range(0f, 1f)] public float idleChance = 0.3f;
    [Range(0f, 1f)] public float dodgeChance = 0.5f;
    [Range(0f, 1f)] public float closeResponse_AttackChance = 0.5f;
    [Range(0f, 1f)] public float closeResponse_MoveAwayChance = 0.3f;
    [Range(0f, 1f)] public float closeResponse_DashAwayChance = 0.2f;
    [Range(0f, 1f)] public float dashInAttackChance = 0.4f;

    private ShapeMovement movement;
    private ShapeAttack attack;
    private Transform playerTransform;
    private float decisionTimer;
    private float attackTimer, jumpTimer, dashTimer;

    [SerializeField] private EnemyState currentState;

    void Start()
    {
        movement = GetComponent<ShapeMovement>();
        attack = GetComponent<ShapeAttack>();
        playerTransform = PlayerManager.Instance.ActivePlayerTransform;

        decisionTimer = decisionInterval;
        currentState = Random.value < idleChance ? EnemyState.Idle : EnemyState.Aggressive;
    }

    void Update()
    {
        if (playerTransform == null) return;

        UpdateTimers();

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0f)
        {
            decisionTimer = decisionInterval;
            MakeDecision();
        }
    }

    void UpdateTimers()
    {
        attackTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;
        dashTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Makes an AI decision each decisionInterval seconds based on player position and internal cooldowns:
    ///
    /// <para>1. Updates movement input to face the player.</para>
    /// <para>2. If the enemy is dodging or retreating, continues that behavior.</para>
    /// <para>3. If the player is within attack range:
    ///   - If not already attacking, randomly choose to attack, move away, or dash away.</para>
    /// <para>4. If the player is attacking and close enough to hit:
    ///   - May dodge based on chance (dash or retreat).</para>
    /// <para>5. If in aggression state:
    ///   - Moves toward the player, attacks when close, then retreats during cooldown.</para>
    /// <para>6. If close but not quite in range:
    ///   - May dash in and attack immediately (chance-based).</para>
    /// <para>7. Otherwise:
    ///   - May idle (stand still and observe) based on idle chance.</para>
    ///
    /// Decisions are made every few frames and constrained by cooldowns and random chance weights.
    /// </summary>
    void MakeDecision()
    {

        Vector2 toPlayer = playerTransform.position - transform.position;
        float xDist = toPlayer.x;
        float absX = Mathf.Abs(xDist);
        bool playerTooClose = absX <= attackRange;
        bool playerInDashRange = absX > attackRange && absX <= dashRange;
        bool playerAbove = toPlayer.y > 1f;

        Vector2 direction = new(Mathf.Sign(xDist), 0f);

        // Check player is attacking and in range
        if (PlayerManager.Instance.IsPlayerAttacking &&
            absX <= attackRange &&
            Random.value < dodgeChance)
        {
            TryDodge(xDist);
            return;
        }

        // Respond to player getting too close (if not already attacking)
        if (playerTooClose && !attack.IsAttacking())
        {
            float roll = Random.value;
            if (roll < closeResponse_AttackChance && attackTimer <= 0f)
            {
                attack.Attack();
                attackTimer = attackCooldown;
                currentState = EnemyState.Retreating;
                return;
            }
            else if (roll < closeResponse_AttackChance + closeResponse_MoveAwayChance)
            {
                movement.SetMoveInputs(-direction);
                return;
            }
            else if (dashTimer <= 0f)
            {
                movement.SetMoveInputs(-direction);
                movement.Dash();
                dashTimer = dashCooldown;
                return;
            }
        }

        // Decision based on current state
        switch (currentState)
        {
            case EnemyState.Idle:
                movement.SetMoveInputs(Vector2.zero);
                if (Random.value > idleChance)
                    currentState = EnemyState.Aggressive;
                break;

            case EnemyState.Aggressive:
                movement.SetMoveInputs(direction);

                if (playerAbove && absX < jumpDistance && jumpTimer <= 0f)
                {
                    movement.Jump();
                    jumpTimer = jumpCooldown;
                    return;
                }

                if (playerInDashRange && Random.value < dashInAttackChance && dashTimer <= 0f)
                {
                    movement.SetMoveInputs(direction);
                    movement.Dash();
                    dashTimer = dashCooldown;
                    if (absX <= attackRange && attackTimer <= 0f)
                    {
                        attack.Attack();
                        attackTimer = attackCooldown;
                        currentState = EnemyState.Retreating;
                    }
                    return;
                }

                if (playerTooClose && attackTimer <= 0f)
                {
                    attack.Attack();
                    attackTimer = attackCooldown;
                    currentState = EnemyState.Retreating;
                    return;
                }

                break;

            case EnemyState.Retreating:
                if (!attack.IsAttacking())
                {
                    movement.SetMoveInputs(-direction);
                    if (attackTimer <= 0.1f)
                        currentState = Random.value < idleChance ? EnemyState.Idle : EnemyState.Aggressive;
                }

                break;
        }
    }

    void TryDodge(float xDist)
    {
        Vector2 away = new(-Mathf.Sign(xDist), 0f);
        movement.SetMoveInputs(away);
        if (dashTimer <= 0f)
        {
            movement.Dash();
            dashTimer = dashCooldown;
        }
    }
}
