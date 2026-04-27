using UnityEngine;

public class PlayerResourceInventory : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private int wood;
    [SerializeField] private int stone;
    [SerializeField] private int special;

    public int Wood => wood;
    public int Stone => stone;
    public int Special => special;

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                wood += amount;
                break;

            case ResourceType.Stone:
                stone += amount;
                break;

            case ResourceType.Special:
                special += amount;
                break;
        }

        Debug.Log($"Added {amount} {type}. Total now: {GetAmount(type)}");
    }

    public bool HasEnough(ResourceType type, int amount)
    {
        return GetAmount(type) >= amount;
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        if (!HasEnough(type, amount))
            return false;

        switch (type)
        {
            case ResourceType.Wood:
                wood -= amount;
                break;

            case ResourceType.Stone:
                stone -= amount;
                break;

            case ResourceType.Special:
                special -= amount;
                break;
        }

        Debug.Log($"Spent {amount} {type}. Total now: {GetAmount(type)}");
        return true;
    }

    public int GetAmount(ResourceType type)
    {
        return type switch
        {
            ResourceType.Wood => wood,
            ResourceType.Stone => stone,
            ResourceType.Special => special,
            _ => 0
        };
    }
}