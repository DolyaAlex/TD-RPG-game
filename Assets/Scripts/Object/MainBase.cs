using UnityEngine;

[RequireComponent(typeof(Health))]
public class MainBase : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private DefeatManager defeatManager;

    public Health Health => health;

    private void Reset()
    {
        health = GetComponent<Health>();
    }

    private void Awake()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    private void OnEnable()
    {
        health.OnDied += HandleBaseDestroyed;
    }

    private void OnDisable()
    {
        health.OnDied -= HandleBaseDestroyed;
    }

    private void HandleBaseDestroyed()
    {
        Debug.Log("Main Base destroyed.");

        if (defeatManager != null)
        {
            defeatManager.TriggerDefeat();
        }
    }
}