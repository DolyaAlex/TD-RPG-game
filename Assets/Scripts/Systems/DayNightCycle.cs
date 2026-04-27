using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Phase Durations")]
    [SerializeField] private float dayDuration = 120f;
    [SerializeField] private float eveningDuration = 20f;
    [SerializeField] private float nightDuration = 90f;

    [Header("Start Settings")]
    [SerializeField] private DayPhase startPhase = DayPhase.Day;
    [SerializeField] private bool startAutomatically = true;

    public DayPhase CurrentPhase { get; private set; }
    public float CurrentPhaseTimeLeft { get; private set; }
    public int CurrentDayNumber { get; private set; } = 1;
    public bool IsRunning { get; private set; }

    public event Action<DayPhase> OnPhaseChanged;
    public event Action OnDayStarted;
    public event Action OnEveningStarted;
    public event Action OnNightStarted;
    public event Action OnNightEnded;

    private void Start()
    {
        SetPhase(startPhase);

        if (startAutomatically)
        {
            IsRunning = true;
        }
    }

    private void Update()
    {
        if (!IsRunning)
            return;

        CurrentPhaseTimeLeft -= Time.deltaTime;

        if (CurrentPhaseTimeLeft <= 0f)
        {
            GoToNextPhase();
        }
    }

    public void StartCycle()
    {
        IsRunning = true;
    }

    public void StopCycle()
    {
        IsRunning = false;
    }

    private void GoToNextPhase()
    {
        switch (CurrentPhase)
        {
            case DayPhase.Day:
                SetPhase(DayPhase.Evening);
                break;

            case DayPhase.Evening:
                SetPhase(DayPhase.Night);
                break;

            case DayPhase.Night:
                OnNightEnded?.Invoke();
                CurrentDayNumber++;
                SetPhase(DayPhase.Day);
                break;
        }
    }

    private void SetPhase(DayPhase newPhase)
    {
        CurrentPhase = newPhase;
        CurrentPhaseTimeLeft = GetDurationForPhase(newPhase);

        Debug.Log($"DayNightCycle: phase changed to {newPhase}. Day: {CurrentDayNumber}");

        OnPhaseChanged?.Invoke(newPhase);

        switch (newPhase)
        {
            case DayPhase.Day:
                OnDayStarted?.Invoke();
                break;

            case DayPhase.Evening:
                OnEveningStarted?.Invoke();
                break;

            case DayPhase.Night:
                OnNightStarted?.Invoke();
                break;
        }
    }

    private float GetDurationForPhase(DayPhase phase)
    {
        switch (phase)
        {
            case DayPhase.Day:
                return dayDuration;

            case DayPhase.Evening:
                return eveningDuration;

            case DayPhase.Night:
                return nightDuration;

            default:
                return dayDuration;
        }
    }

    public float GetCurrentPhaseDuration()
    {
        return GetDurationForPhase(CurrentPhase);
    }

    public float GetCurrentPhaseProgress01()
    {
        float duration = GetCurrentPhaseDuration();

        if (duration <= 0f)
            return 1f;

        return 1f - Mathf.Clamp01(CurrentPhaseTimeLeft / duration);
    }
}