using UnityEngine;

public class BankRepository : Repository
{
    private const string _key = "Coins";
    private int _coinsAmount;

    public static int CoinsAmount { get; private set; }

    public override void Initialize()
    {
        _coinsAmount = (PlayerPrefs.HasKey(_key)) ? PlayerPrefs.GetInt(_key) : 0;
        CoinsAmount = _coinsAmount;
    }

    public void SetCoins(int value)
    {
        _coinsAmount = value;
        CoinsAmount = _coinsAmount;

        Save(_key, _coinsAmount);
    }

    protected override void Save(string key, int value)
    {
        PlayerPrefs.SetInt(_key, _coinsAmount);
    }
}