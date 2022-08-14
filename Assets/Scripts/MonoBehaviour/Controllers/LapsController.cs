using UnityEngine;
using ProjectCar.UI;
using ProjectCar.Interactors;

namespace ProjectCar
{
    namespace Controllers
    {
        public sealed class LapsController : Controller
        {
            [SerializeField] private DisplayTextUpdater _lapsTextUpdater;
            [SerializeField] private CheckPoint[] _checkPoints;
            [SerializeField] private LapTimer _lapTimer;
            [SerializeField][Range(1, 10)] private int _maxLapsAmount;
            private LapsInteractor _lapsInteractor;
            private FallsInteractor _fallsInteractor;
            private BankInteractor _bankInteractor;
            private int _checkPointsCompleted;

            [field: SerializeField] public TrainingObject Training—onfig { get; private set; }

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
                    _bankInteractor.AddCoins(10);
                }
                else
                {
                    _lapsInteractor?.IncreaseLapsAmount();

                    if (Training—onfig != null)
                    {
                        if (Training—onfig.IsCalledFromAnotherScript && !Training—onfig.IsTriggerPassed)
                        {
                            Training—onfig.TrainingDescriptionDisplay();
                        }
                    }
                }
            }
        }
    }
}
