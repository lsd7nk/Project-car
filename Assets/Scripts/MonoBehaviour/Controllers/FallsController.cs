using UnityEngine;
using ProjectCar.UI;
using ProjectCar.Handlers;
using ProjectCar.Interactors;

namespace ProjectCar
{
    namespace Controllers
    {
        public sealed class FallsController : Controller
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
                    _lapsInteractor?.ResetLapsAmount();
                }
                else
                {
                    _interactor?.FallsAmountIncrease();
                    _lapsInteractor.OnFallEvent?.Invoke();
                }

                _bankInteractor?.SubsractCoins(penalty);
                _lapTimer?.ResetTime();
            }

            private void SetFallsText() => _text.SetText($"Falls amount: {_interactor.FallsAmount}");

            private int CalculatePenaltyForFall() => (int) Mathf.Pow(_interactor.FallsAmount, 2);
        }
    }
}
