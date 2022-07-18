using UnityEngine;

public class LapsController : Controller
{
    [SerializeField] private DisplayTextUpdater _lapsTextUpdater;
    [SerializeField] private CheckPoint[] _checkPoints;
    [SerializeField] private LapTimer _lapTimer;
    private LapsInteractor _lapsInteractor;
    private FallsInteractor _fallsInteractor;
    private BankInteractor _bankInteractor;
    private int _checkPointsCompleted;
    private const int _maxLapsAmount = 10;

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
        _fallsInteractor = base.Initialize<FallsInteractor>();
        _bankInteractor = base.Initialize<BankInteractor>();

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
            ChangeLapsAmount();
            _lapTimer?.ResetTime();
        }
        else
        {
            if (point.Index != _checkPointsCompleted) { return; }

            if (++_checkPointsCompleted == _checkPoints.Length + 1)
            {
                ChangeLapsAmount();
            }
        }
    }

    private void ChangeLapsAmount()
    {
        _checkPointsCompleted = 1;

        if (_lapsInteractor.LapsCompletedAmount >= _maxLapsAmount)
        {
            _lapsInteractor.ResetLapsAmount();
            _fallsInteractor.ResetFallsAmount();
            _bankInteractor.AddCoins(_maxLapsAmount);
        }
        else
        {
            _lapsInteractor?.IncreaseLapsAmount();
        }
    }
}
