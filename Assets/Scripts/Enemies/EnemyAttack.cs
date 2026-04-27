using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Type")]
    [SerializeField] private EnemyAttackType attackType = EnemyAttackType.Melee;

    [Header("Attack Stats")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("Ranged Placeholder")]
    [SerializeField] private bool useDirectDamageForRanged = true;

    private float lastAttackTime = -999f;

    public EnemyAttackType AttackType => attackType;
    public float AttackRange => attackRange;

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public bool IsTargetInRange(Health targetHealth)
    {
        if (targetHealth == null)
            return false;

        Vector3 selfPosition = transform.position;
        selfPosition.y = 0f;

        Collider targetCollider = targetHealth.GetComponent<Collider>();

        Vector3 targetPoint;

        if (targetCollider != null)
        {
            targetPoint = targetCollider.ClosestPoint(transform.position);
        }
        else
        {
            targetPoint = targetHealth.transform.position;
        }

        targetPoint.y = 0f;

        float distance = Vector3.Distance(selfPosition, targetPoint);
        return distance <= attackRange;
    }

    public void Attack(IDamageable target)
    {
        if (target == null || !CanAttack())
            return;

        lastAttackTime = Time.time;

        switch (attackType)
        {
            case EnemyAttackType.Melee:
                PerformMeleeAttack(target);
                break;

            case EnemyAttackType.Ranged:
                PerformRangedAttack(target);
                break;
        }
    }

    private void PerformMeleeAttack(IDamageable target)
    {
        target.TakeDamage(damage);
        Debug.Log($"{name} performed MELEE attack for {damage} damage.");
    }

    private void PerformRangedAttack(IDamageable target)
    {
        if (useDirectDamageForRanged)
        {
            target.TakeDamage(damage);
            Debug.Log($"{name} performed RANGED placeholder attack for {damage} damage.");
        }
        else
        {
            Debug.LogWarning($"{name}: Ranged attack selected, but projectile logic is not implemented yet.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = attackType == EnemyAttackType.Melee ? Color.red : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}