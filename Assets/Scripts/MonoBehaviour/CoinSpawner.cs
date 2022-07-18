using UnityEngine;
using UnityEngine.Events;

public sealed class CoinSpawner : MonoBehaviour
{
    [SerializeField] private Coin _coinPrefab;
    [SerializeField] private Transform[] _coinsPoints;
    private LapsInteractor _lapsInteractor;
    private PoolMonoBehaviour<Coin> _pool;
    private UnityAction _callback;

    public void Initialize(UnityAction callback)
    {
        if (_coinPrefab == null) { return; }

        _lapsInteractor = GameController.Instance.InteractorsBase.GetInteractor<LapsInteractor>();
        _pool = new PoolMonoBehaviour<Coin>(_coinPrefab, transform, _coinsPoints.Length);
        _callback = callback;

        _lapsInteractor.OnChangeLapsAmountEvent += AllocateCoinsByPoints;

        SetCallbackByCoinEvent();
        AllocateCoinsByPoints();
    }

    private void OnDisable()
    {
        _lapsInteractor.OnChangeLapsAmountEvent -= AllocateCoinsByPoints;
    }

    private void SetCallbackByCoinEvent()
    {
        foreach (var coin in _pool.Pool)
        {
            coin.OnMoneyCollectionEvent.AddListener(_callback);
        }
    }

    private void AllocateCoinsByPoints(int lapsAmount = 10)
    {
        if (_coinsPoints == null) { return; }

        int coinsAmount = Random.Range(1, lapsAmount);
        _pool.TurnOffObjects();

        foreach (Transform point in _coinsPoints)
        {
            if (coinsAmount-- <= 0) { return; }

            if (Random.Range(0, 100) % 2 == 0)
            {
                Coin coin = _pool.GetObject(true);

                coin.transform.position = point.position;
            }
        }
    }
}
