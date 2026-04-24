using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        UpdateText();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (playerHealth == null || healthText == null)
            return;

        healthText.text = $"Player HP: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
    }
}