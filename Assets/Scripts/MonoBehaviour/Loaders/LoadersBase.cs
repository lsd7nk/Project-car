using UnityEngine;

namespace ProjectCar
{
    namespace Loaders
    {
        public sealed class LoadersBase : MonoBehaviour, ILoader
        {
            [SerializeField] private CarLoader _carLoader;
            [SerializeField] private MaterialLoader _materialLoader;
            [SerializeField] private GameObjectLoader[] _objectsLoader;

            public void Load()
            {
                if (_objectsLoader == null) { return; }

                foreach (GameObjectLoader loader in _objectsLoader)
                {
                    loader?.Load();
                }
            }

            public void Set()
            {
                if (_objectsLoader == null) { return; }

                foreach (GameObjectLoader loader in _objectsLoader)
                {
                    loader?.Set();
                }
            }

            private void Awake() => InitializeLoaders();

            private void CarInitialize()
            {
                _carLoader?.Initialize();
                _materialLoader?.Initialize();

                _carLoader?.Load();
                _materialLoader?.Load();

                _carLoader?.Set();
                _materialLoader?.Set();
            }

            private void InitializeLoaders()
            {
                CarInitialize();
                Load();
                Set();
            }
        }
    }
}
