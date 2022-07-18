using UnityEngine;

public class FallsController : Controller
{
    [SerializeField] private DisplayTextUpdater _text;
    [SerializeField] private FallsHandler _handler;
    [SerializeField] private LapTimer _lapTimer;
    private FallsInteractor _interactor;
    private LapsInteractor _lapsInteractor;
    private BankInteractor _bankInteractor;
    private const int _maxFallsAmount = 10;

    public void Initialize()
    {
        _interactor = base.Initialize<FallsInteractor>();
        _lapsInteractor = base.Initialize<LapsInteractor>();
        _bankInteractor = base.Initialize<BankInteractor>();

        _handler.Initialize();
        _text.Initialize();
        SetFallsText();

        _handler.OnFallHandleEvent += HandleFall;
        _interactor.OnChangeFallsAmountEvent += SetFallsText;
    }

    private void OnDisable()
    {
        _handler.OnFallHandleEvent -= HandleFall;
        _interactor.OnChangeFallsAmountEvent -= SetFallsText;
    }

    private void HandleFall()
    {
        int penalty = CalculatePenaltyForFall();

        if (_interactor.FallsAmount >= _maxFallsAmount)
        {
            _interactor?.ResetFallsAmount();
        }
        else
        {
            _interactor?.FallsAmountIncrease();
        }

        _bankInteractor?.SubsractCoins(penalty);
        _lapsInteractor?.ResetLapsAmount();
        _lapTimer?.ResetTime();
    }

    private void SetFallsText() => _text.SetText($"Falls amount: {_interactor.FallsAmount}");

    private int CalculatePenaltyForFall()
    {
        int penalty, completedLapsAmount, fallsAmount;

        penalty = (int)Mathf.Pow(_interactor.FallsAmount, 2);
        completedLapsAmount = _lapsInteractor.LapsCompletedAmount;
        fallsAmount = _interactor.FallsAmount;

        return (completedLapsAmount > fallsAmount) ? _maxFallsAmount : penalty;
    }
}
