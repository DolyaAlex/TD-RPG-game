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

    [Header("Rotation")]
    [SerializeField] private float attackRotationSpeed = 20f;

    private float lastAttackTime = -999f;

    public bool IsAttackingThisFrame { get; private set; }
    public Vector3 AimDirection { get; private set; } = Vector3.forward;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        IsAttackingThisFrame = false;

        UpdateAimDirection();

        if (inputReader == null)
            return;

        if (!inputReader.AttackPressedThisFrame)
            return;

        TryAttack();
    }

    private void UpdateAimDirection()
    {
        if (mainCamera == null || Mouse.current == null)
            return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (!groundPlane.Raycast(ray, out float enter))
            return;

        Vector3 hitPoint = ray.GetPoint(enter);
        Vector3 direction = hitPoint - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            AimDirection = direction.normalized;
        }
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        IsAttackingThisFrame = true;

        RotateToAimDirection();

        Vector3 center = attackPoint != null
            ? attackPoint.position
            : transform.position + AimDirection * attackRange;

        Collider[] hits = Physics.OverlapSphere(center, attackRadius, hittableLayers, QueryTriggerInteraction.Collide);

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

    private void RotateToAimDirection()
    {
        if (AimDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(AimDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            attackRotationSpeed * Time.deltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 direction = AimDirection.sqrMagnitude > 0.001f ? AimDirection : transform.forward;
        Vector3 center = attackPoint != null
            ? attackPoint.position
            : transform.position + direction * attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}