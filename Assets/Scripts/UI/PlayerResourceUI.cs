using TMPro;
using UnityEngine;

public class PlayerResourceUI : MonoBehaviour
{
    [SerializeField] private PlayerResourceInventory inventory;
    [SerializeField] private TextMeshProUGUI resourcesText;

    private void Update()
    {
        if (inventory == null || resourcesText == null)
            return;

        resourcesText.text =
            $"Wood: {inventory.Wood}\n" +
            $"Stone: {inventory.Stone}\n" +
            $"Special: {inventory.Special}";
    }
}