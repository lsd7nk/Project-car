using System;
using UnityEngine;

public sealed class TimeInvoker : MonoBehaviour
{
    [HideInInspector] public event Action OnSecondTimeUpdateEvent;
    private float _oneSecondTime;

    public bool IsPaused => PauseManager.Instance.IsPaused;

    public void ResetSecondsAmount() => _oneSecondTime = 0f;

    private void Update()
    {
        if (IsPaused) { return; }

        CountSecond();
    }

    private void CountSecond()
    {
        _oneSecondTime += Time.deltaTime;

        if (_oneSecondTime >= 1f)
        {
            _oneSecondTime -= 1f;
            OnSecondTimeUpdateEvent?.Invoke();
        }
    }
}
