using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Camera mainCamera;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRadius = 0.7f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask hittableLayers;

    [Header("Cursor Targeting")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float maxMouseRayDistance = 1000f;

    private float lastAttackTime = -999f;
    private Vector3 lastAttackDirection = Vector3.forward;

    public bool IsAttackingThisFrame { get; private set; }
    public Vector3 CurrentAttackDirection => lastAttackDirection;

    private void Update()
    {
        IsAttackingThisFrame = false;

        if (!inputReader.AttackPressed)
            return;

        inputReader.ConsumeAttack();
        TryAttack();
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        if (!TryGetMouseWorldPoint(out Vector3 mouseWorldPoint))
            return;

        Vector3 direction = mouseWorldPoint - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        lastAttackDirection = direction.normalized;
        IsAttackingThisFrame = true;
        lastAttackTime = Time.time;

        transform.forward = lastAttackDirection;

        Vector3 center = attackPoint != null
            ? attackPoint.position
            : transform.position + lastAttackDirection * attackRange;

        Collider[] hits = Physics.OverlapSphere(center, attackRadius, hittableLayers);

        bool hitSomething = false;

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
                continue;

            damageable.TakeDamage(attackDamage);
            hitSomething = true;
        }

        Debug.Log(hitSomething ? "Attack hit target." : "Attack missed.");
    }

    private bool TryGetMouseWorldPoint(out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return false;

        if (Mouse.current == null)
            return false;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxMouseRayDistance, groundLayerMask))
        {
            worldPoint = hit.point;
            return true;
        }

        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float enter))
        {
            worldPoint = ray.GetPoint(enter);
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 direction = lastAttackDirection.sqrMagnitude > 0.001f
            ? lastAttackDirection
            : transform.forward;

        Vector3 center = attackPoint != null
            ? attackPoint.position
            : transform.position + direction * attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}