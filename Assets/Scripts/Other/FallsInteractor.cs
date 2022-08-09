using System;

public class FallsInteractor : Interactor
{
    public event Action OnChangeFallsAmountEvent;
    private FallsRepository _repository;

    public override Repository Repository => _repository;
    public int FallsAmount => _repository.FallsAmount;

    public override void Initialize()
    {
        _repository = RepositoryInitialize<FallsRepository>();
    }

    public void FallsAmountIncrease()
    {
        _repository.SetFalls(FallsAmount + 1);
        OnChangeFallsAmountEvent?.Invoke();
    }

    public void ResetFallsAmount()
    {
        _repository.ResetFalls();
        OnChangeFallsAmountEvent?.Invoke();
    }
}
