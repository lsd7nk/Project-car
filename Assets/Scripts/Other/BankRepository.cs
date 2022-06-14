using UnityEngine;

public class BankRepository : Repository
{
    private const string _key = "Coins";

    public int CoinsAmount { get; private set; }

    public override void Initialize()
    {
        CoinsAmount = (PlayerPrefs.HasKey(_key)) ? PlayerPrefs.GetInt(_key) : 0;
    }

    public void SetCoins(int value)
    {
        CoinsAmount = value;

        Save(_key, CoinsAmount);
    }

    protected override void Save(string key, int value)
    {
        PlayerPrefs.SetInt(_key, CoinsAmount);
    }
}