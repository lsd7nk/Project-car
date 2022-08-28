using UnityEngine;
using ProjectCar.Interactors;

namespace ProjectCar
{
    namespace Loaders
    {
        public sealed class CarLoader : MonoBehaviour, ILoader
        {
            [SerializeField] private Transform _spawnPoint;
            [SerializeField] private string _pathToFolder;
            private GameObject _loadedObject;

            public GameObject InstantiatedObject { get; private set; }
            public GameObject SpoilerObject { get; private set; }
            public CarInteractor CarInteractor {get; private set; }
            public SpoilerInteractor SpoilerInteractor { get; private set; }
            private string _path => _pathToFolder + CarInteractor.CarName;

            public void Initialize()
            {
                CarInteractor = new CarInteractor();
                SpoilerInteractor = new SpoilerInteractor();

                SpoilerInteractor.Initialize();
                CarInteractor.Initialize();
            }

            public void Load()
            {
                if (string.IsNullOrEmpty(_path)) { return; }

                _loadedObject = Resources.Load<GameObject>(_path);
            }

            public void Set()
            {
                if (_loadedObject == null || _spawnPoint == null) { return; }

                InstantiatedObject = Instantiate(_loadedObject, _spawnPoint.position, _spawnPoint.rotation);
                SpoilerObject = InstantiatedObject.transform.GetChild(1).gameObject;

                SetSpoiler();
            }

            public void TurnOnSpoiler()
            {
                SpoilerInteractor.EquipSpoiler();
                SpoilerObject.SetActive(true);
            }

            public void TurnOffSpoiler()
            {
                SpoilerInteractor.UnequipSpoiler();
                SpoilerObject.SetActive(false);
            }

            private void SetSpoiler()
            {
                if (SpoilerObject == null) { return; }

                if (SpoilerInteractor.CurrentState == "Equipped")
                {
                    TurnOnSpoiler();
                }
                else
                {
                    TurnOffSpoiler();
                }
            }
        }
    }
}
