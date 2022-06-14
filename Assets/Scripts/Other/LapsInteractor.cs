public class LapsInteractor : Interactor
{
    public override Repository Repository => null;
    public int LapsCompletedAmount { get; private set; }

    public override void Initialize()
    {
        LapsCompletedAmount = 0;
    }

    public void IncreaseLapsAmount() { LapsCompletedAmount++; }
}
