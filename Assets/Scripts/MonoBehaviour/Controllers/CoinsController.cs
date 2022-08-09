using UnityEngine;

public class CoinsController : Controller
{
    [SerializeField] private DisplayTextUpdater _coinsTextUpdater;
    [SerializeField] private CoinSpawner _spawner;
    private BankInteractor _bankInteractor;

    public void Initialize()
    {
        _bankInteractor = base.Initialize<BankInteractor>();
        _coinsTextUpdater.Initialize($"{_bankInteractor.CoinsAmount}$");
        _spawner.Initialize(() => { _bankInteractor?.AddCoins(1); });

        _bankInteractor.OnChangeCoinsAmountEvent += SetCoinsAmountOnDisplay;
    }

    private void SetCoinsAmountOnDisplay()
    {
        _coinsTextUpdater?.SetText($"{_bankInteractor.CoinsAmount}$");
    }
}
