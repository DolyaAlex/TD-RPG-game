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

    [Header("Flat Range Check")]
    [SerializeField] private float verticalSearchHalfHeight = 50f;
    [SerializeField] private bool drawFlatRangeGizmo = true;

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

        Collider[] hits = Physics.OverlapBox(
            origin,
            new Vector3(attackRange, verticalSearchHalfHeight, attackRange),
            Quaternion.identity,
            enemyLayers,
            QueryTriggerInteraction.Collide
        );

        Debug.Log($"{name}: possible targets in vertical search box = {hits.Length}");

        Health closestTarget = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (Collider hit in hits)
        {
            Health health = hit.GetComponentInParent<Health>();

            if (health == null || health.IsDead)
                continue;

            Vector3 targetPoint = GetFlatTargetPoint(hit, origin);

            float distanceSqr = GetFlatDistanceSqr(origin, targetPoint);

            if (distanceSqr > attackRange * attackRange)
                continue;

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

        Collider targetCollider = target.GetComponentInChildren<Collider>();

        Vector3 targetPoint = targetCollider != null
            ? GetFlatTargetPoint(targetCollider, origin)
            : target.transform.position;

        float distanceSqr = GetFlatDistanceSqr(origin, targetPoint);

        return distanceSqr <= attackRange * attackRange;
    }

    private Vector3 GetFlatTargetPoint(Collider targetCollider, Vector3 origin)
    {
        Vector3 flatOrigin = origin;
        flatOrigin.y = targetCollider.bounds.center.y;

        return targetCollider.ClosestPoint(flatOrigin);
    }

    private float GetFlatDistanceSqr(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return (a - b).sqrMagnitude;
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
        if (!drawFlatRangeGizmo)
            return;

        Vector3 origin = GetOriginPosition();

        Gizmos.color = Color.blue;
        DrawFlatCircle(origin, attackRange);

        Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
        Gizmos.DrawWireCube(
            origin,
            new Vector3(attackRange * 2f, verticalSearchHalfHeight * 2f, attackRange * 2f)
        );
    }

    private void DrawFlatCircle(Vector3 center, float radius)
    {
        const int segments = 64;

        Vector3 previousPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI * 2f;

            Vector3 nextPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius
            );

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}