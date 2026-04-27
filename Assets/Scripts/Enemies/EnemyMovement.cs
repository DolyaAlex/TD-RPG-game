using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float angularSpeed = 720f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float destinationUpdateThreshold = 0.25f;

    private NavMeshAgent agent;
    private Vector3 lastDestination;
    private float lastStoppingDistance = -1f;
    private bool hasDestination;

    public NavMeshAgent Agent => agent;
    public bool HasDestination => hasDestination;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
        agent.updateRotation = true;
        agent.updateUpAxis = true;
        agent.autoBraking = true;
        agent.avoidancePriority = Random.Range(20, 80);
    }

    public void SetDestination(Vector3 destination, float stoppingDistance)
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        bool stoppingDistanceChanged = Mathf.Abs(lastStoppingDistance - stoppingDistance) > 0.01f;
        bool destinationChanged = !hasDestination || Vector3.Distance(lastDestination, destination) > destinationUpdateThreshold;

        if (!stoppingDistanceChanged && !destinationChanged)
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
}