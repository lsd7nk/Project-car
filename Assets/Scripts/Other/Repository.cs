public abstract class Repository
{
    public abstract void Initialize();
    protected abstract void Save(string key, int value);
}
