using UnityEngine;

[RequireComponent(typeof(EnemyRTSMovement))]
public class EnemySeparation : MonoBehaviour
{
    [Header("Separation")]
    [SerializeField] private float separationRadius = 1.1f;
    [SerializeField] private float separationStrength = 2.5f;
    [SerializeField] private LayerMask unitLayers;

    [Header("Player Avoidance")]
    [SerializeField] private Transform player;
    [SerializeField] private float playerRadius = 0.6f;
    [SerializeField] private float playerWeight = 1.5f;

    private EnemyRTSMovement movement;

    private void Awake()
    {
        movement = GetComponent<EnemyRTSMovement>();
    }

    private void Start()
    {
        if (player == null)
        {
            PlayerMover playerMover = FindFirstObjectByType<PlayerMover>();
            if (playerMover != null)
            {
                player = playerMover.transform;
            }
        }
    }

    private void Update()
    {
        Vector3 offset = CalculateUnitSeparation() + CalculatePlayerSeparation();
        movement.AddSeparationOffset(offset * separationStrength);
    }

    private Vector3 CalculateUnitSeparation()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            separationRadius,
            unitLayers,
            QueryTriggerInteraction.Collide
        );

        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (Collider hit in hits)
        {
            if (hit.transform == transform)
                continue;

            Vector3 away = transform.position - hit.transform.position;
            away.y = 0f;

            float distance = away.magnitude;
            if (distance < 0.001f)
                continue;

            float weight = 1f - Mathf.Clamp01(distance / separationRadius);
            separation += away.normalized * weight;
            count++;
        }

        if (count == 0)
            return Vector3.zero;

        separation /= count;
        return separation;
    }

    private Vector3 CalculatePlayerSeparation()
    {
        if (player == null)
            return Vector3.zero;

        Vector3 away = transform.position - player.position;
        away.y = 0f;

        float distance = away.magnitude;
        float checkRadius = separationRadius + playerRadius;

        if (distance > checkRadius || distance < 0.001f)
            return Vector3.zero;

        float weight = 1f - Mathf.Clamp01(distance / checkRadius);
        return away.normalized * weight * playerWeight;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}