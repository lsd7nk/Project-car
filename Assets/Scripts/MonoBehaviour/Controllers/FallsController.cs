using UnityEngine;

public class FallsController : Controller
{
    [SerializeField] private DisplayTextUpdater _text;
    [SerializeField] private FallsHandler _handler;
    [SerializeField] private LapTimer _lapTimer;
    private FallsInteractor _interactor;
    private LapsInteractor _lapsInteractor;
    private const int _maxFallsAmount = 10;

    public void Initialize()
    {
        _interactor = base.Initialize<FallsInteractor>();
        _lapsInteractor = base.Initialize<LapsInteractor>();

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
        if (_interactor.FallsAmount >= _maxFallsAmount)
        {
            _interactor?.ResetFallsAmount();
        }
        else
        {
            _interactor?.FallsAmountIncrease();
        }

        _lapsInteractor?.ResetLapsAmount();
        _lapTimer?.ResetTime();
    }

    private void SetFallsText() => _text.SetText($"Falls amount: {_interactor.FallsAmount}");
}
