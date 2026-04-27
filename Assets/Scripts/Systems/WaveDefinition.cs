using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WaveDefinition",
    menuName = "TD RPG/Waves/Wave Definition"
)]
public class WaveDefinition : ScriptableObject
{
    [Header("Wave Info")]
    [SerializeField] private string waveName = "New Wave";
    [TextArea]
    [SerializeField] private string warningText = "Enemies are approaching!";

    [Header("Spawn Settings")]
    [SerializeField] private float delayBeforeWave = 2f;
    [SerializeField] private WaveEnemyGroup[] enemyGroups;

    public string WaveName => waveName;
    public string WarningText => warningText;
    public float DelayBeforeWave => delayBeforeWave;
    public WaveEnemyGroup[] EnemyGroups => enemyGroups;
}

[Serializable]
public class WaveEnemyGroup
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int count = 3;
    [SerializeField] private float delayBetweenSpawns = 0.5f;
    [SerializeField] private float delayAfterGroup = 1f;

    public GameObject EnemyPrefab => enemyPrefab;
    public int Count => count;
    public float DelayBetweenSpawns => delayBetweenSpawns;
    public float DelayAfterGroup => delayAfterGroup;
}