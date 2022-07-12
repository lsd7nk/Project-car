using UnityEngine;

public class LapsController : Controller
{
    [SerializeField] private DisplayTextUpdater _lapsTextUpdater;
    [SerializeField] private CheckPoint[] _checkPoints;
    [SerializeField] private LapTimer _lapTimer;
    private LapsInteractor _lapsInteractor;
    private int _checkPointsCompleted;

    public void Initialize()
    {
        InitializeLapsInteractor();
        _lapsTextUpdater.Initialize();
        _lapTimer.Initialize();

        foreach (CheckPoint point in _checkPoints)
        {
            point.OnCheckPointComplete.AddListener(HandleCurrentCheckPoint);
        }
    }

    private void InitializeLapsInteractor()
    {
        _lapsInteractor = base.Initialize<LapsInteractor>();

        _lapsInteractor.OnChangeLapsAmountEvent += (int lapsAmount) =>
        {
            _lapsTextUpdater?.SetText($"Laps completed: {lapsAmount}");
        };
        _lapsInteractor.OnResetLapsAmountEvent += (int lapsAmount) =>
        {
            _checkPointsCompleted = 1;
        };
    }

    private void HandleCurrentCheckPoint(CheckPoint point)
    {
        if (point.Index == 0 && _checkPointsCompleted == _checkPoints.Length)
        {
            IncreaseLapsAmount();
            _lapTimer?.ResetTime();
        }
        else
        {
            if (point.Index != _checkPointsCompleted) { return; }

            if (++_checkPointsCompleted == _checkPoints.Length + 1)
            {
                IncreaseLapsAmount();
            }
        }
    }

    private void IncreaseLapsAmount()
    {
        _checkPointsCompleted = 1;
        _lapsInteractor?.IncreaseLapsAmount();
    }
}
