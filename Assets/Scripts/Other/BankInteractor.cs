public class BankInteractor : Interactor
{
    private BankRepository _repository;

    public override Repository Repository => _repository;

    public override void Initialize()
    {
        _repository = new BankRepository();
        _repository.Initialize();
    }
}