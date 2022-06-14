using UnityEngine;

public class LapsController : Controller
{
    [SerializeField] private DisplayTextUpdater _lapsTextUpdater;
    [SerializeField] private CheckPoint[] _checkPoints;
    private LapsInteractor _lapsInteractor;
    private int _checkPointsCompleted;

    public void Initialize()
    {
        _lapsInteractor = base.Initialize<LapsInteractor>();
        _lapsTextUpdater.Initialize();
        _checkPointsCompleted = 0;

        foreach (CheckPoint point in _checkPoints)
        {
            point.OnCheckPointComplete.AddListener(HandleCurrentCheckPoint);
        }
    }

    private void HandleCurrentCheckPoint(CheckPoint point)
    {
        if (point.Index == 0 && _checkPointsCompleted == _checkPoints.Length)
        {
            IncreaseLapsAmount();
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
        _lapsTextUpdater?.SetText("Laps completed: " + _lapsInteractor.LapsCompletedAmount.ToString());
    }
}
