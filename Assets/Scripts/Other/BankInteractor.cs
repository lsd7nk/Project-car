using System;

public class BankInteractor : Interactor
{
    public event Action OnChangeCoinsAmountEvent;
    private BankRepository _repository;

    public override Repository Repository => _repository;
    
    public int CoinsAmount => _repository.CoinsAmount;

    public override void Initialize()
    {
        _repository = RepositoryInitialize<BankRepository>();
    }

    public void AddCoins(int value)
    {
        if (value < 0) { return; }

        _repository.SetCoins(CoinsAmount + value);
        OnChangeCoinsAmountEvent?.Invoke();
    }

    public void SubsractCoins(int value)
    {
        if (value < 0) { return; }

        if ((CoinsAmount - value) >= 0)
        {
            _repository.SetCoins(CoinsAmount - value);
            OnChangeCoinsAmountEvent?.Invoke();
        }
    }
}