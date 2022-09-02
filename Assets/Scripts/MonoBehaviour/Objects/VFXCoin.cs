using UnityEngine;
using UnityEngine.Events;

namespace ProjectCar
{
    namespace Objects
    {
        [RequireComponent(typeof(Coin))]
        public sealed class VFXCoin : MonoBehaviour
        {
            [SerializeField] private GameObject _liftingCoinObj;
            private ParticleSystem _liftingCoinVFX;
            private Coin _coin;

            private void Awake() => _coin = GetComponent<Coin>();

            private void OnEnable() => _coin.OnMoneyCollectionEvent.AddListener(PlayLiftingCoinVFX);

            private void OnDisable() => _coin.OnMoneyCollectionEvent.RemoveListener(PlayLiftingCoinVFX);

            private void PlayLiftingCoinVFX()
            {
                if (_liftingCoinObj == null) { return; }

                GameObject vfxObj;

                vfxObj = Instantiate(_liftingCoinObj, _coin.transform.position, _liftingCoinObj.transform.rotation);
                _liftingCoinVFX = vfxObj.GetComponent<ParticleSystem>();

                _liftingCoinVFX.Play();
            }
        }
    }
}
