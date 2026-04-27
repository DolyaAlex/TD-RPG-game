using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Setup")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Input")]
    [SerializeField] private Key spawnKey = Key.H;
    [SerializeField] private bool allowKeyboardTestSpawn = true;

    [Header("Spawn Count")]
    [SerializeField] private int spawnCount = 3;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool chooseRandomSpawnPoint = true;

    [Header("Spawn Radius")]
    [SerializeField] private float minSpawnRadius = 2f;
    [SerializeField] private float maxSpawnRadius = 5f;

    [Header("Spawn Validation")]
    [SerializeField] private float spawnCheckRadius = 0.75f;
    [SerializeField] private int maxAttemptsPerEnemy = 20;
    [SerializeField] private LayerMask blockingLayers;

    [Header("NavMesh Validation")]
    [SerializeField] private float navMeshSampleDistance = 2f;

    private void Update()
    {
        if (!allowKeyboardTestSpawn)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current[spawnKey].wasPressedThisFrame)
        {
            SpawnEnemies(spawnCount);
        }
    }

    public void SpawnEnemies(int count)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: Enemy prefab is not assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: No spawn points assigned.");
            return;
        }

        int spawned = 0;

        for (int i = 0; i < count; i++)
        {
            Transform spawnCenter = GetSpawnPointForEnemy(i);

            if (spawnCenter == null)
            {
                Debug.LogWarning("EnemySpawner: Spawn point is null.");
                continue;
            }

            if (TryGetSpawnPosition(spawnCenter.position, out Vector3 spawnPosition))
            {
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                spawned++;
            }
            else
            {
                Debug.LogWarning($"EnemySpawner: Could not find valid spawn point for enemy #{i + 1} near {spawnCenter.name}.");
            }
        }

        Debug.Log($"EnemySpawner: Spawned {spawned}/{count} enemies.");
    }

    private Transform GetSpawnPointForEnemy(int enemyIndex)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return null;

        if (chooseRandomSpawnPoint)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex];
        }

        int clampedIndex = enemyIndex % spawnPoints.Length;
        return spawnPoints[clampedIndex];
    }

    private bool TryGetSpawnPosition(Vector3 centerPosition, out Vector3 spawnPosition)
    {
        for (int attempt = 0; attempt < maxAttemptsPerEnemy; attempt++)
        {
            Vector2 direction = Random.insideUnitCircle;

            if (direction.sqrMagnitude < 0.001f)
            {
                direction = Vector2.right;
            }

            direction.Normalize();
            float distance = Random.Range(minSpawnRadius, maxSpawnRadius);

            Vector3 candidate = centerPosition + new Vector3(direction.x, 0f, direction.y) * distance;

            if (!TrySampleOnNavMesh(candidate, out Vector3 navMeshPosition))
                continue;

            if (!IsPositionFree(navMeshPosition))
                continue;

            spawnPosition = navMeshPosition;
            return true;
        }

        spawnPosition = default;
        return false;
    }

    private bool TrySampleOnNavMesh(Vector3 candidate, out Vector3 navMeshPosition)
    {
        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            navMeshPosition = hit.position;
            return true;
        }

        navMeshPosition = default;
        return false;
    }

    private bool IsPositionFree(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(
            position,
            spawnCheckRadius,
            blockingLayers,
            QueryTriggerInteraction.Ignore
        );

        return hits.Length == 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return;

        foreach (Transform point in spawnPoints)
        {
            if (point == null)
                continue;

            Vector3 center = point.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, minSpawnRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, maxSpawnRadius);
        }
    }
}