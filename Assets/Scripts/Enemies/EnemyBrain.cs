using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemyRTSMovement))]
[RequireComponent(typeof(EnemyAttack))]
public class EnemyBrain : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private EnemyArchetype archetype = EnemyArchetype.Regular;
    [SerializeField] private MainBase mainBase;

    private Health health;
    private EnemyRTSMovement movement;
    private EnemyAttack attack;

    private EnemyState currentState = EnemyState.Idle;

    public EnemyArchetype Archetype => archetype;
    public EnemyState CurrentState => currentState;

    private void Awake()
    {
        health = GetComponent<Health>();
        movement = GetComponent<EnemyRTSMovement>();
        attack = GetComponent<EnemyAttack>();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDied += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDied -= HandleDeath;
        }
    }

    private void Start()
    {
        if (mainBase == null)
        {
            mainBase = FindFirstObjectByType<MainBase>();
        }
    }

    private void Update()
    {
        if (health == null || health.IsDead)
            return;

        switch (archetype)
        {
            case EnemyArchetype.Regular:
                TickRegular();
                break;

            case EnemyArchetype.Neutral:
                TickNeutralPlaceholder();
                break;

            case EnemyArchetype.Destroyer:
                TickDestroyerPlaceholder();
                break;

            case EnemyArchetype.Aggressive:
                TickAggressivePlaceholder();
                break;
        }
    }

    private void TickRegular()
    {
        if (mainBase == null || mainBase.Health == null || mainBase.Health.IsDead)
        {
            currentState = EnemyState.Idle;
            movement.ClearDestination();
            return;
        }

        Health baseHealth = mainBase.Health;

        if (attack.IsTargetInRange(baseHealth))
        {
            currentState = EnemyState.Attacking;
            movement.ClearDestination();
            FaceTarget(mainBase.transform.position);
            attack.Attack(baseHealth);
        }
        else
        {
            currentState = EnemyState.MovingToTarget;
            movement.SetDestination(mainBase.transform.position, GetDesiredStoppingDistance());
        }
    }

    private float GetDesiredStoppingDistance()
    {
        switch (attack.AttackType)
        {
            case EnemyAttackType.Melee:
                return Mathf.Max(0.1f, attack.AttackRange * 0.9f);

            case EnemyAttackType.Ranged:
                return Mathf.Max(0.1f, attack.AttackRange * 0.85f);

            default:
                return attack.AttackRange;
        }
    }

    private void TickNeutralPlaceholder()
    {
        currentState = EnemyState.Idle;
        movement.ClearDestination();
    }

    private void TickDestroyerPlaceholder()
    {
        TickRegular();
    }

    private void TickAggressivePlaceholder()
    {
        TickRegular();
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
            15f * Time.deltaTime
        );
    }

    private void HandleDeath()
    {
        currentState = EnemyState.Dead;
        movement.ClearDestination();

        Debug.Log($"{name} died.");
        Destroy(gameObject);
    }
}