public abstract class Interactor
{
    public abstract Repository Repository { get; }
    public abstract void Initialize();
    public T RepositoryInitialize<T>() where T : Repository, new()
    {
        var repository = new T();
        repository.Initialize();

        return repository;
    }
}
