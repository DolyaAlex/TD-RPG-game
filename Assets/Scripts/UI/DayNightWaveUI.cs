using TMPro;
using UnityEngine;

public class DayNightWaveUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private WaveManager waveManager;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("Warning")]
    [SerializeField] private float warningDuration = 5f;

    private float warningTimer;

    private void Awake()
    {
        if (dayNightCycle == null)
        {
            dayNightCycle = FindFirstObjectByType<DayNightCycle>();
        }

        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }
    }

    private void OnEnable()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveWarning += HandleWaveWarning;
            waveManager.OnWaveStarted += HandleWaveStarted;
            waveManager.OnWaveCompleted += HandleWaveCompleted;
            waveManager.OnAllWavesCompleted += HandleAllWavesCompleted;
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveWarning -= HandleWaveWarning;
            waveManager.OnWaveStarted -= HandleWaveStarted;
            waveManager.OnWaveCompleted -= HandleWaveCompleted;
            waveManager.OnAllWavesCompleted -= HandleAllWavesCompleted;
        }
    }

    private void Start()
    {
        HideWarning();
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
        UpdateWarningTimer();
    }

    private void UpdateUI()
    {
        if (dayNightCycle != null)
        {
            if (phaseText != null)
            {
                phaseText.text =
                    $"Day {dayNightCycle.CurrentDayNumber}: {GetPhaseName(dayNightCycle.CurrentPhase)}";
            }

            if (timerText != null)
            {
                int secondsLeft = Mathf.CeilToInt(dayNightCycle.CurrentPhaseTimeLeft);
                timerText.text = $"Time: {FormatTime(secondsLeft)}";
            }
        }

        if (waveManager != null && waveText != null)
        {
            if (waveManager.AreAllWavesCompleted)
            {
                waveText.text = "Waves: Completed";
            }
            else if (waveManager.IsWaveActive)
            {
                waveText.text =
                    $"Wave {waveManager.CurrentWaveNumber}/{waveManager.TotalWaves} | Enemies: {waveManager.AliveEnemies}";
            }
            else
            {
                int nextWave = Mathf.Clamp(
                    waveManager.CurrentWaveNumber + 1,
                    1,
                    Mathf.Max(1, waveManager.TotalWaves)
                );

                waveText.text = $"Next Wave: {nextWave}/{waveManager.TotalWaves}";
            }
        }
    }

    private void UpdateWarningTimer()
    {
        if (warningText == null)
            return;

        if (warningTimer <= 0f)
            return;

        warningTimer -= Time.deltaTime;

        if (warningTimer <= 0f)
        {
            HideWarning();
        }
    }

    private void HandleWaveWarning(WaveDefinition wave, int waveNumber)
    {
        if (warningText == null || wave == null)
            return;

        warningText.gameObject.SetActive(true);
        warningText.text = $"Warning: Wave {waveNumber}\n{wave.WarningText}";
        warningTimer = warningDuration;
    }

    private void HandleWaveStarted(WaveDefinition wave, int waveNumber)
    {
        if (warningText == null || wave == null)
            return;

        warningText.gameObject.SetActive(true);
        warningText.text = $"Wave {waveNumber} started!\n{wave.WaveName}";
        warningTimer = warningDuration;
    }

    private void HandleWaveCompleted(WaveDefinition wave, int waveNumber)
    {
        if (warningText == null)
            return;

        warningText.gameObject.SetActive(true);
        warningText.text = $"Wave {waveNumber} cleared!";
        warningTimer = warningDuration;
    }

    private void HandleAllWavesCompleted()
    {
        if (warningText == null)
            return;

        warningText.gameObject.SetActive(true);
        warningText.text = "All waves completed!";
        warningTimer = 999f;
    }

    private void HideWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

        warningTimer = 0f;
    }

    private string GetPhaseName(DayPhase phase)
    {
        switch (phase)
        {
            case DayPhase.Day:
                return "Day";

            case DayPhase.Evening:
                return "Evening";

            case DayPhase.Night:
                return "Night";

            default:
                return phase.ToString();
        }
    }

    private string FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return $"{minutes:00}:{seconds:00}";
    }
}