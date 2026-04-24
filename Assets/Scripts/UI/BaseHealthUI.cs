using TMPro;
using UnityEngine;

public class BaseHealthUI : MonoBehaviour
{
    [SerializeField] private Health baseHealth;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        UpdateText();
    }

    private void OnEnable()
    {
        if (baseHealth != null)
        {
            baseHealth.OnHealthChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (baseHealth != null)
        {
            baseHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (baseHealth == null || healthText == null)
            return;

        healthText.text = $"Base HP: {baseHealth.CurrentHealth}/{baseHealth.MaxHealth}";
    }
}