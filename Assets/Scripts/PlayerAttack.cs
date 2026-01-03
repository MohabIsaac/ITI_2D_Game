using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 10;

    public BoxCollider2D attackHitBox;
    private Vector2 originalOffset;

    private bool m_attacking = false;
    private HashSet<Bandit> enemiesHitThisAttack = new HashSet<Bandit>();

    void Awake()
    {
        originalOffset = attackHitBox.offset;
    }

    void Start()
    {
        attackHitBox.enabled = false;
    }

    // ⚠️ Call this using an Animation Event
    public void StartAttack()
    {
        m_attacking = true;
        enemiesHitThisAttack.Clear();
        attackHitBox.enabled = true;
    }

    // ⚠️ Call this using an Animation Event
    public void EndAttack()
    {
        m_attacking = false;
        attackHitBox.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_attacking) return;

        Bandit enemy = other.GetComponent<Bandit>();
        if (enemy == null) return;

        if (enemiesHitThisAttack.Contains(enemy)) return;

        enemiesHitThisAttack.Add(enemy);
        enemy.TakeDamage(attackDamage);

        Debug.Log("Hit Enemy");
    }
    public void SetFacingDirection(int dir)
    {
        attackHitBox.offset = new Vector2(
            Mathf.Abs(originalOffset.x) * dir,
            originalOffset.y
        );
    }
}
