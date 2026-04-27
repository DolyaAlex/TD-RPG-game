using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyRTSMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float destinationUpdateThreshold = 0.25f;

    [Header("NavMesh Constrain")]
    [SerializeField] private float navMeshSampleDistance = 0.75f;

    private NavMeshAgent agent;
    private Vector3 lastDestination;
    private float lastStoppingDistance = -1f;
    private bool hasDestination;

    private Vector3 separationOffset;

    public bool HasDestination => hasDestination;
    public Vector3 DesiredVelocity => agent != null ? agent.desiredVelocity : Vector3.zero;
    public float RemainingDistance => agent != null ? agent.remainingDistance : 0f;
    public bool HasPath => agent != null && agent.hasPath;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = true;
        agent.autoBraking = true;
        agent.speed = moveSpeed;
        agent.avoidancePriority = Random.Range(20, 80);
    }

    private void Update()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        Vector3 desired = agent.desiredVelocity;
        desired.y = 0f;

        Vector3 finalVelocity = desired + separationOffset;
        finalVelocity.y = 0f;

        if (finalVelocity.magnitude > moveSpeed)
        {
            finalVelocity = finalVelocity.normalized * moveSpeed;
        }

        Vector3 currentPosition = transform.position;
        Vector3 desiredNextPosition = currentPosition + finalVelocity * Time.deltaTime;

        Vector3 finalPosition = currentPosition;

        if (finalVelocity.sqrMagnitude > 0.0001f)
        {
            if (NavMesh.SamplePosition(desiredNextPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                finalPosition = hit.position;
            }
        }

        transform.position = finalPosition;

        Vector3 movementDelta = finalPosition - currentPosition;
        movementDelta.y = 0f;

        if (movementDelta.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDelta.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        agent.nextPosition = transform.position;
        separationOffset = Vector3.zero;
    }

    public void SetDestination(Vector3 destination, float stoppingDistance)
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        bool stoppingChanged = Mathf.Abs(lastStoppingDistance - stoppingDistance) > 0.01f;
        bool destinationChanged = !hasDestination || Vector3.Distance(lastDestination, destination) > destinationUpdateThreshold;

        if (!stoppingChanged && !destinationChanged)
            return;

        agent.stoppingDistance = stoppingDistance;
        agent.isStopped = false;
        agent.SetDestination(destination);

        lastDestination = destination;
        lastStoppingDistance = stoppingDistance;
        hasDestination = true;
    }

    public void ClearDestination()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        agent.isStopped = true;
        agent.ResetPath();
        hasDestination = false;
    }

    public void AddSeparationOffset(Vector3 offset)
    {
        separationOffset += offset;
    }
}