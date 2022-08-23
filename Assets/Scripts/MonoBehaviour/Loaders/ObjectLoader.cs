using UnityEngine;

namespace ProjectCar
{
    namespace Loaders
    {
        public sealed class GameObjectLoader : MonoBehaviour, ILoader
        {
            [SerializeField] private Transform _spawnPoint;
            [SerializeField] private string _path;
            private GameObject _loadedObject;

            public void Load()
            {
                if (string.IsNullOrEmpty(_path)) { return; }

                _loadedObject = Resources.Load<GameObject>(_path);
            }

            public void Set()
            {
                if (_loadedObject == null || _spawnPoint == null) { return; }

                Instantiate(_loadedObject, _spawnPoint.position, Quaternion.identity);
            }
        }
    }
}
