using ProjectCar.Repositories;

namespace ProjectCar
{
    namespace Interactors
    {
        public sealed class LapsInteractor : Interactor
        {
            public override Repository Repository => null;
            public delegate void OnChangeLapsAmount(int lapsAmount = 0);
            public OnChangeLapsAmount OnChangeLapsAmountEvent;
            public OnChangeLapsAmount OnResetLapsAmountEvent;

            public int LapsCompletedAmount { get; private set; }

            public override void Initialize() => ResetLapsAmount();

            public void ResetLapsAmount()
            {
                LapsCompletedAmount = 0;
                OnResetLapsAmountEvent?.Invoke();
                OnChangeLapsAmountEvent?.Invoke(LapsCompletedAmount);
            }

            public void IncreaseLapsAmount()
            {
                LapsCompletedAmount++;
                OnChangeLapsAmountEvent?.Invoke(LapsCompletedAmount);
            }
        }
    }
}
