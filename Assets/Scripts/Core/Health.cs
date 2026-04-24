using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead => hasDied;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    private bool hasDied;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (hasDied)
            return;

        if (damage <= 0)
            return;

        CurrentHealth -= damage;

        if (CurrentHealth < 0)
            CurrentHealth = 0;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        Debug.Log($"{name} took {damage} damage. HP: {CurrentHealth}/{maxHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (hasDied)
            return;

        if (amount <= 0)
            return;

        CurrentHealth += amount;

        if (CurrentHealth > maxHealth)
            CurrentHealth = maxHealth;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void RestoreFullHealth()
    {
        hasDied = false;
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        if (hasDied)
            return;

        hasDied = true;
        OnDied?.Invoke();
    }
}