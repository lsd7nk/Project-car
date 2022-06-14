using UnityEngine;

public class CoinsController : Controller
{
    [SerializeField] private DisplayTextUpdater _coinsTextUpdater;
    [SerializeField] private Coin _coinPrefab;
    [SerializeField] private Transform[] _coinsPoints;
    private PoolMonoBehaviour<Coin> _pool;
    private BankInteractor _bankInteractor;

    public void Initialize()
    {
        if (_coinPrefab == null) { return; }

        _pool = new PoolMonoBehaviour<Coin>(_coinPrefab, transform, 3);
        _bankInteractor = base.Initialize<BankInteractor>();

        AllocateCoinsByPoints();
        _coinsTextUpdater?.Initialize(_bankInteractor.CoinsAmount.ToString() + "$");
    }

    private void AllocateCoinsByPoints()
    {
        if (_coinsPoints == null) { return; }

        foreach (Transform point in _coinsPoints)
        {
            Coin coin = _pool.GetObject(true);

            coin.transform.position = point.position;
            coin.OnMoneyCollection.AddListener(AdditionCoin);
        }
    }

    private void AdditionCoin()
    {
        _bankInteractor?.AdditionCoins(1);
        _coinsTextUpdater?.SetText(_bankInteractor.CoinsAmount.ToString() + "$");
    }
}
