public class BankInteractor : Interactor
{
    private BankRepository _repository;

    public override Repository Repository => _repository;
    
    public int CoinsAmount => _repository.CoinsAmount;

    public override void Initialize()
    {
        _repository = RepositoryInitialize<BankRepository>();
    }

    public void AdditionCoins(int value)
    {
        if (value < 0) { return; }

        _repository.SetCoins(CoinsAmount + value);
    }
}