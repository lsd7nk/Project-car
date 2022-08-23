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
            public CarInteractor Interactor {get; private set; }
            private string _path => _pathToFolder + Interactor.CarName;

            public void Initialize()
            {
                Interactor = new CarInteractor();
                Interactor.Initialize();
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
            }
        }
    }
}
