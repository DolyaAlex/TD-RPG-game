using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuildableRuin : MonoBehaviour, IInteractable
{
    [Header("Build Cost")]
    [SerializeField] private ResourceType costType = ResourceType.Wood;
    [SerializeField] private int costAmount = 5;

    [Header("Build Result")]
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Transform buildPoint;

    [Header("Settings")]
    [SerializeField] private bool destroyRuinAfterBuild = true;

    public void Interact(GameObject interactor)
    {
        if (towerPrefab == null)
        {
            Debug.LogWarning($"{name}: towerPrefab is not assigned.");
            return;
        }

        PlayerResourceInventory inventory = interactor.GetComponent<PlayerResourceInventory>();

        if (inventory == null)
        {
            inventory = interactor.GetComponentInParent<PlayerResourceInventory>();
        }

        if (inventory == null)
        {
            Debug.LogWarning($"{name}: interactor has no PlayerResourceInventory.");
            return;
        }

        if (!inventory.SpendResource(costType, costAmount))
        {
            Debug.Log($"Not enough {costType} to restore {name}. Need: {costAmount}");
            return;
        }

        Transform spawnPoint = buildPoint != null ? buildPoint : transform;

        Instantiate(towerPrefab, spawnPoint.position, spawnPoint.rotation);

        Debug.Log($"{name} restored for {costAmount} {costType}.");

        if (destroyRuinAfterBuild)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}