using UnityEngine;

public class FallsController : Controller
{
    [SerializeField] private DisplayTextUpdater _text;
    [SerializeField] private FallsHandler _handler;
    private FallsInteractor _interactor;
    private LapTimer _lapTimer;
    private LapsInteractor _lapsInteractor;

    public void Initialize()
    {
        _interactor = base.Initialize<FallsInteractor>();
        _lapsInteractor = base.Initialize<LapsInteractor>();
        _lapTimer = GetComponent<LapTimer>();

        _text?.Initialize();
        SetFallsText();

        _handler.OnFallHandleEvent += FallHandle;
        _interactor.OnChangeFallsAmountEvent += SetFallsText;
    }

    private void OnDisable()
    {
        _handler.OnFallHandleEvent -= FallHandle;
        _interactor.OnChangeFallsAmountEvent -= SetFallsText;
    }

    private void FallHandle()
    {
        _interactor.FallsAmountIncrease();
        _lapsInteractor.ResetLapsAmount();
        _lapTimer.ResetTime();
    }

    private void SetFallsText()
    {
        _text.SetText($"Falls amount: {_interactor.FallsAmount}");
    }
}
