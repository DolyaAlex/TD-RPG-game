using UnityEngine;

public class TreeResourceNode : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Drop Settings")]
    [SerializeField] private ResourcePickup resourcePickupPrefab;
    [SerializeField] private int minDrops = 2;
    [SerializeField] private int maxDrops = 4;
    [SerializeField] private int amountPerDrop = 1;
    [SerializeField] private float dropRadius = 1.2f;

    private int currentHealth;
    private bool isDestroyed;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed)
            return;

        currentHealth -= damage;

        Debug.Log($"{name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            BreakTree();
        }
    }

    private void BreakTree()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;
        SpawnDrops();
        Destroy(gameObject);
    }

    private void SpawnDrops()
    {
        if (resourcePickupPrefab == null)
        {
            Debug.LogWarning("TreeResourceNode: resourcePickupPrefab is not assigned.");
            return;
        }

        int dropCount = Random.Range(minDrops, maxDrops + 1);

        for (int i = 0; i < dropCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * dropRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0.25f, randomCircle.y);

            ResourcePickup pickup = Instantiate(resourcePickupPrefab, spawnPosition, Quaternion.identity);
            pickup.Initialize(ResourceType.Wood, amountPerDrop);
        }
    }
}