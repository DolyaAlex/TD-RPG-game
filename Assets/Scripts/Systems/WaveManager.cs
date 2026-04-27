using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Waves")]
    [SerializeField] private WaveDefinition[] waves;
    [SerializeField] private bool startWaveAutomaticallyAtNight = true;

    public int CurrentWaveIndex { get; private set; } = -1;
    public int CurrentWaveNumber => CurrentWaveIndex + 1;
    public int TotalWaves => waves != null ? waves.Length : 0;
    public bool IsWaveActive { get; private set; }
    public bool AreAllWavesCompleted { get; private set; }
    public int AliveEnemies { get; private set; }

    public WaveDefinition CurrentWave
    {
        get
        {
            if (waves == null)
                return null;

            if (CurrentWaveIndex < 0 || CurrentWaveIndex >= waves.Length)
                return null;

            return waves[CurrentWaveIndex];
        }
    }

    public event Action<WaveDefinition, int> OnWaveWarning;
    public event Action<WaveDefinition, int> OnWaveStarted;
    public event Action<WaveDefinition, int> OnWaveCompleted;
    public event Action OnAllWavesCompleted;

    private Coroutine waveRoutine;

    private void Awake()
    {
        if (dayNightCycle == null)
        {
            dayNightCycle = FindFirstObjectByType<DayNightCycle>();
        }

        if (enemySpawner == null)
        {
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
        }
    }

    private void OnEnable()
    {
        if (dayNightCycle != null)
        {
            dayNightCycle.OnEveningStarted += HandleEveningStarted;
            dayNightCycle.OnNightStarted += HandleNightStarted;
        }
    }

    private void OnDisable()
    {
        if (dayNightCycle != null)
        {
            dayNightCycle.OnEveningStarted -= HandleEveningStarted;
            dayNightCycle.OnNightStarted -= HandleNightStarted;
        }
    }

    private void Update()
    {
        if (!IsWaveActive)
            return;

        if (waveRoutine != null)
            return;

        if (AliveEnemies <= 0)
        {
            CompleteCurrentWave();
        }
    }

    public void StartNextWave()
    {
        if (IsWaveActive)
        {
            Debug.LogWarning("WaveManager: Wave is already active.");
            return;
        }

        if (AreAllWavesCompleted)
        {
            Debug.Log("WaveManager: All waves already completed.");
            return;
        }

        int nextIndex = CurrentWaveIndex + 1;

        if (waves == null || nextIndex >= waves.Length)
        {
            CompleteAllWaves();
            return;
        }

        CurrentWaveIndex = nextIndex;
        WaveDefinition wave = waves[CurrentWaveIndex];

        if (wave == null)
        {
            Debug.LogWarning($"WaveManager: Wave #{CurrentWaveNumber} is null. Skipping.");
            CompleteCurrentWave();
            return;
        }

        waveRoutine = StartCoroutine(SpawnWaveRoutine(wave));
    }

    private void HandleEveningStarted()
    {
        if (AreAllWavesCompleted)
            return;

        int nextWaveIndex = CurrentWaveIndex + 1;

        if (waves == null || nextWaveIndex >= waves.Length)
            return;

        WaveDefinition nextWave = waves[nextWaveIndex];

        if (nextWave == null)
            return;

        OnWaveWarning?.Invoke(nextWave, nextWaveIndex + 1);

        Debug.Log($"WaveManager: Warning for wave #{nextWaveIndex + 1}: {nextWave.WarningText}");
    }

    private void HandleNightStarted()
    {
        if (!startWaveAutomaticallyAtNight)
            return;

        StartNextWave();
    }

    private IEnumerator SpawnWaveRoutine(WaveDefinition wave)
    {
        IsWaveActive = true;
        AliveEnemies = 0;

        OnWaveStarted?.Invoke(wave, CurrentWaveNumber);

        Debug.Log($"WaveManager: Wave #{CurrentWaveNumber} started: {wave.WaveName}");

        if (wave.DelayBeforeWave > 0f)
        {
            yield return new WaitForSeconds(wave.DelayBeforeWave);
        }

        WaveEnemyGroup[] groups = wave.EnemyGroups;

        if (groups != null)
        {
            for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
            {
                WaveEnemyGroup group = groups[groupIndex];

                if (group == null)
                    continue;

                if (group.EnemyPrefab == null)
                {
                    Debug.LogWarning($"WaveManager: Enemy prefab is missing in wave {wave.WaveName}.");
                    continue;
                }

                for (int enemyIndex = 0; enemyIndex < group.Count; enemyIndex++)
                {
                    GameObject enemy = enemySpawner.SpawnEnemy(group.EnemyPrefab, enemyIndex);

                    if (enemy != null)
                    {
                        RegisterEnemy(enemy);
                    }

                    if (group.DelayBetweenSpawns > 0f)
                    {
                        yield return new WaitForSeconds(group.DelayBetweenSpawns);
                    }
                }

                if (group.DelayAfterGroup > 0f)
                {
                    yield return new WaitForSeconds(group.DelayAfterGroup);
                }
            }
        }

        waveRoutine = null;

        if (AliveEnemies <= 0)
        {
            CompleteCurrentWave();
        }
    }

    private void RegisterEnemy(GameObject enemy)
    {
        Health health = enemy.GetComponent<Health>();

        if (health == null)
        {
            Debug.LogWarning($"WaveManager: Spawned enemy {enemy.name} has no Health component.");
            return;
        }

        AliveEnemies++;
        health.OnDied += HandleEnemyDied;
    }

    private void HandleEnemyDied()
    {
        AliveEnemies--;

        if (AliveEnemies < 0)
        {
            AliveEnemies = 0;
        }
    }

    private void CompleteCurrentWave()
    {
        if (!IsWaveActive)
            return;

        IsWaveActive = false;

        WaveDefinition completedWave = CurrentWave;

        Debug.Log($"WaveManager: Wave #{CurrentWaveNumber} completed.");

        OnWaveCompleted?.Invoke(completedWave, CurrentWaveNumber);

        if (waves == null || CurrentWaveIndex >= waves.Length - 1)
        {
            CompleteAllWaves();
        }
    }

    private void CompleteAllWaves()
    {
        if (AreAllWavesCompleted)
            return;

        AreAllWavesCompleted = true;
        IsWaveActive = false;

        Debug.Log("WaveManager: All waves completed. Victory condition can be triggered here.");

        OnAllWavesCompleted?.Invoke();
    }
}