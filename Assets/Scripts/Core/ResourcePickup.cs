using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourcePickup : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int amount = 1;

    public void Initialize(ResourceType type, int value)
    {
        resourceType = type;
        amount = value;
    }

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerResourceInventory inventory = other.GetComponent<PlayerResourceInventory>();

        if (inventory == null)
        {
            inventory = other.GetComponentInParent<PlayerResourceInventory>();
        }

        if (inventory == null)
            return;

        inventory.AddResource(resourceType, amount);
        Destroy(gameObject);
    }
}