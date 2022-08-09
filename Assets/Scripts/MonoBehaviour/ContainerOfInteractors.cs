public sealed class ContainerOfInteractors
{
    public BankInteractor BankInteractor { get; private set; }
    public LapsInteractor LapsInteractor { get; private set; }
    public FallsInteractor FallsInteractor { get; private set; }

    public ContainerOfInteractors()
    {
        BankInteractor = GetInteractor<BankInteractor>();
        LapsInteractor = GetInteractor<LapsInteractor>();
        FallsInteractor = GetInteractor<FallsInteractor>();
    }

    private T GetInteractor<T>() where T : Interactor
    {
        return GameController.Instance.InteractorsBase.GetInteractor<T>();
    }
}
