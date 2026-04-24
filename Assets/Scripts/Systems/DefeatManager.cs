using TMPro;
using UnityEngine;

public class DefeatManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private TextMeshProUGUI defeatText;

    public bool IsDefeatTriggered { get; private set; }

    private void Start()
    {
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(false);
        }

        if (defeatText != null)
        {
            defeatText.text = "Defeat";
        }

        Time.timeScale = 1f;
    }

    public void TriggerDefeat()
    {
        if (IsDefeatTriggered)
            return;

        IsDefeatTriggered = true;

        Debug.Log("Defeat triggered.");

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }

        if (defeatText != null)
        {
            defeatText.text = "Base Destroyed\nDefeat";
        }

        Time.timeScale = 0f;
    }
}