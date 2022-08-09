using UnityEngine;
using System;

[RequireComponent(typeof(TimeInvoker))]
public sealed class LapTimer : MonoBehaviour
{
    private DisplayTextUpdater _lapTimerUpdater;
    private TimeInvoker _timeInvoker;

    public int TotalSecondsPerLap { get; private set; }

    public void Initialize()
    {
        _lapTimerUpdater = GetComponent<DisplayTextUpdater>();
        _timeInvoker = GetComponent<TimeInvoker>();

        _timeInvoker.OnSecondTimeUpdateEvent += TotalSecondsIncrease;
        _lapTimerUpdater?.Initialize();
    }

    public void ResetTime()
    {
        TotalSecondsPerLap = 0;

        _timeInvoker.ResetSecondsAmount();
        _lapTimerUpdater?.SetText($"{SecondsToTimeSpan()}");
    }

    private void OnDisable() => _timeInvoker.OnSecondTimeUpdateEvent -= TotalSecondsIncrease;

    private void TotalSecondsIncrease()
    {
        ++TotalSecondsPerLap;
        _lapTimerUpdater?.SetText($"{SecondsToTimeSpan()}");
    }

    private TimeSpan SecondsToTimeSpan()
    {
        int minutes = TotalSecondsPerLap / 60;
        int seconds = TotalSecondsPerLap - minutes * 60;
        
        return new TimeSpan(0, minutes, seconds);
    }
}
