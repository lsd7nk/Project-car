using UnityEngine;

public class FallsRepository : Repository
{
    private const string _key = "FallsAmount";

    public int FallsAmount { get; private set; }

    public override void Initialize()
    {
        FallsAmount = (PlayerPrefs.HasKey(_key)) ? PlayerPrefs.GetInt(_key) : 0;
    }

    public void SetFalls(int value)
    {
        FallsAmount = value;
        Save(_key, FallsAmount);
    }

    public void ResetFalls()
    {
        FallsAmount = 0;
        Save(_key, FallsAmount);
    }
}
