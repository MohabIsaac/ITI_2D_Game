using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float m_speed = 4f;
    [SerializeField] private float m_jumpForce = 7.5f;

    [Header("Patrol Settings")]
    [SerializeField] private float flipPause = 0.5f; // pause at edges/walls
    private bool isPaused = false;
    private float pauseTimer = 0f;

    [Header("Player Detection")]
    public Transform player;
    [SerializeField] private float reactionTime = 2f; // pause before chasing
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float closeRange = 1.5f; // play close-range animation
    private bool chasingPlayer = false;
    private bool reactingToPlayer = false;
    private float reactionTimer = 0f;
    public bool inAttackRange = false;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float prepTime = 0.5f; // optional prep before attack
    private bool isAttacking = false;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    [SerializeField] private float hurtDuration = 0.5f; // how long enemy is frozen when hurt
    public bool isHurt = false;


    [Header("Sensors")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;

    private Rigidbody2D m_body2d;
    private SpriteRenderer sr;
    private Animator m_animator;
    private Sensor_Bandit m_groundSensor;

    private bool movingRight = true;
    private bool m_grounded = false;
    private bool m_combatIdle = false;

    private float wallCheckOffsetX;
    private float lastFlipTime;
    private float flipCooldown = 0.1f;

    void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        currentHealth = maxHealth;

        if (wallCheck != null)
            wallCheckOffsetX = wallCheck.localPosition.x;

        sr.flipX = movingRight;
    }

    void FixedUpdate()
    {
        // === STOP ALL MOVEMENT IF ATTACKING ===
        if (isAttacking || isHurt || currentHealth <= 0)
        {
            m_body2d.linearVelocity = Vector2.zero;
            return;
        }

        // === PLAYER DETECTION & CHASE ===
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= detectionRange)
            {
                // Reaction pause before chasing
                if (!chasingPlayer && !reactingToPlayer)
                {
                    reactingToPlayer = true;
                    reactionTimer = reactionTime;
                    m_body2d.linearVelocity = Vector2.zero;
                    m_animator.SetInteger("AnimState", 0);
                }

                if (reactingToPlayer)
                {
                    reactionTimer -= Time.fixedDeltaTime;
                    if (reactionTimer <= 0f)
                    {
                        reactingToPlayer = false;
                        chasingPlayer = true;
                    }
                    return;
                }

                // Attack if in range
                if (distance <= attackRange && !isAttacking)
                {
                    StartCoroutine(AttackCoroutine());
                    return;
                }

                // Close-range animation
                else if (distance <= closeRange && !isAttacking && currentHealth > 0)
                {
                    m_body2d.linearVelocity = Vector2.zero;
                    m_animator.SetInteger("AnimState", 3); // close-range idle/prep
                    return;
                }

                // Chase player
                if (chasingPlayer && !isAttacking)
                {
                    ChasePlayer();
                    return;
                }
            }
            else
            {
                chasingPlayer = false;
                reactingToPlayer = false;
            }
        }

        // === PATROL ===
        if (isPaused)
        {
            pauseTimer -= Time.fixedDeltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                Flip();
            }
            return;
        }

        Patrol();
    }

    public void TakeDamage(int amount)
    {
        if (isHurt) return;

        isHurt = true;
        currentHealth -= amount;
        m_animator.SetTrigger("Hurt");

        Invoke(nameof(ResetHurt), 0.3f);

        if (currentHealth <= 0)
            Die();
    }
    void ResetHurt()
    {
        isHurt = false;
    }

    private IEnumerator HurtCoroutine()
    {
        isHurt = true;

        // Stop movement
        m_body2d.linearVelocity = Vector2.zero;

        // Trigger hurt animation
        m_animator.SetTrigger("Hurt");

        // Wait for hurt duration
        yield return new WaitForSeconds(hurtDuration);

        isHurt = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only take damage from the active attack collider
        if (other.CompareTag("PlayerAttack"))
        {
            PlayerAttack attack = other.GetComponent<PlayerAttack>();
            if (attack != null && other.enabled) // collider active
            {
                TakeDamage(attack.attackDamage);
            }
        }
    }

    private void Die()
    {
        isAttacking = false;
        isHurt = false;
        m_body2d.linearVelocity = Vector2.zero;
        m_animator.SetTrigger("Death");

        // Optional: destroy after animation
        Destroy(gameObject, 1f);
    }


    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        // Stop moving
        m_body2d.linearVelocity = Vector2.zero;

        // Prep animation
        if (prepTime > 0f)
        {
            m_animator.SetInteger("AnimState", 3); // prep
            yield return new WaitForSeconds(prepTime);
        }

        // Trigger actual attack
        if(currentHealth > 0)
        m_animator.SetTrigger("Attack");

        // Wait for cooldown
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void ChasePlayer()
    {
        float direction = player.position.x > transform.position.x ? 1 : -1;
        m_body2d.linearVelocity = new Vector2(direction * chaseSpeed, m_body2d.linearVelocity.y);

        // Flip sprite toward player
        if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
            FlipImmediate();
    }

    void FlipImmediate()
    {
        movingRight = !movingRight;
        sr.flipX = !sr.flipX;

        if (wallCheck != null)
        {
            Vector3 pos = wallCheck.localPosition;
            pos.x = -wallCheck.localPosition.x;
            wallCheck.localPosition = pos;
        }
    }

    void Patrol()
    {
        m_body2d.linearVelocity = new Vector2((movingRight ? 1 : -1) * m_speed, m_body2d.linearVelocity.y);

        // Ground check
        bool groundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.5f, groundLayer);
        bool wallAhead = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, 0.3f, groundLayer);

        if (!groundAhead || wallAhead)
        {
            isPaused = true;
            pauseTimer = flipPause;
            m_body2d.linearVelocity = Vector2.zero;
            return;
        }
    }

    void Flip()
    {
        if (Time.time - lastFlipTime < flipCooldown) return;
        lastFlipTime = Time.time;

        movingRight = !movingRight;
        sr.flipX = !sr.flipX;

        if (wallCheck != null)
        {
            Vector3 pos = wallCheck.localPosition;
            pos.x = -wallCheckOffsetX;
            wallCheck.localPosition = pos;
        }

        m_body2d.linearVelocity = new Vector2((movingRight ? 1 : -1) * m_speed, m_body2d.linearVelocity.y);
    }

    void Update()
    {
        // Grounded logic
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", true);
        }
        else if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", false);
        }

        // Attack cooldown handled in coroutine
        // Set animation states only if not attacking
        if (!isAttacking)
        {
            float speedX = Mathf.Abs(m_body2d.linearVelocity.x);
            if (speedX > 0.1f)
                m_animator.SetInteger("AnimState", 2); // walk
            else if (m_combatIdle)
                m_animator.SetInteger("AnimState", 1); // combat idle
            else
                m_animator.SetInteger("AnimState", 0); // idle
        }
    }

    void OnDrawGizmos()
    {
        if (!groundCheck || !wallCheck) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position,
            wallCheck.position + (movingRight ? Vector3.right : Vector3.left) * 0.3f);
    }
}
