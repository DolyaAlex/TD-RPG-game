using UnityEngine;

public class DefenseTower : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float targetRefreshInterval = 0.25f;
    [SerializeField] private bool rotateToTarget = true;
    [SerializeField] private float rotationSpeed = 10f;

    private float lastAttackTime = -999f;
    private float lastTargetRefreshTime = -999f;

    private Health currentTarget;

    private void Update()
    {
        RefreshTargetIfNeeded();

        if (currentTarget == null || currentTarget.IsDead)
            return;

        if (!IsTargetInRange(currentTarget))
        {
            currentTarget = null;
            return;
        }

        if (rotateToTarget)
        {
            FaceTarget(currentTarget.transform.position);
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            currentTarget.TakeDamage(damage);
            Debug.Log($"{name} attacked {currentTarget.name} for {damage} damage.");
        }
    }

    private void RefreshTargetIfNeeded()
    {
        if (Time.time < lastTargetRefreshTime + targetRefreshInterval)
            return;

        lastTargetRefreshTime = Time.time;
        currentTarget = FindClosestTarget();
    }

    private Health FindClosestTarget()
    {
        Vector3 origin = GetOriginPosition();

        Collider[] hits = Physics.OverlapSphere(
            origin,
            attackRange,
            enemyLayers,
            QueryTriggerInteraction.Collide
        );

        Debug.Log($"{name}: targets in range = {hits.Length}");

        Health closestTarget = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (Collider hit in hits)
        {
            Health health = hit.GetComponentInParent<Health>();

            if (health == null || health.IsDead)
                continue;

            Vector3 targetPoint = hit.ClosestPoint(origin);

            origin.y = 0f;
            targetPoint.y = 0f;

            float distanceSqr = (targetPoint - origin).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestTarget = health;
            }
        }

        return closestTarget;
    }

    private bool IsTargetInRange(Health target)
    {
        if (target == null)
            return false;

        Vector3 origin = GetOriginPosition();
        Collider targetCollider = target.GetComponent<Collider>();

        Vector3 targetPoint = targetCollider != null
            ? targetCollider.ClosestPoint(origin)
            : target.transform.position;

        origin.y = 0f;
        targetPoint.y = 0f;

        float distance = Vector3.Distance(origin, targetPoint);
        return distance <= attackRange;
    }

    private Vector3 GetOriginPosition()
    {
        return attackOrigin != null ? attackOrigin.position : transform.position;
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GetOriginPosition(), attackRange);
    }
}