using UnityEngine;
using ProjectCar.Interactors;

namespace ProjectCar
{
    namespace Loaders
    {
        public class MaterialLoader : MonoBehaviour, ILoader
        {
            [SerializeField] private CarLoader _carLoader;
            [SerializeField] private string _pathToFolder;
            private Material _loadedMaterial;
            private MeshRenderer _meshRenderer;
            private CarMaterialInteractor _interactor;

            private string CarName => _carLoader.CarInteractor.CarName;
            private string _path => _pathToFolder + CarName +  "/" + _interactor.MaterialName;

            public void Initialize()
            {
                _interactor = new CarMaterialInteractor();
                _interactor.Initialize();
            }

            public void Load()
            {
                if (string.IsNullOrEmpty(_path)) { return; }

                _loadedMaterial = Resources.Load<Material>(_path);
            }

            public void Set()
            {
                if (_loadedMaterial == null) { return; }

                if (_carLoader != null)
                {
                    if (_meshRenderer == null)
                    {
                        _meshRenderer = _carLoader.InstantiatedObject.GetComponentInChildren<MeshRenderer>();
                    }
                }
                _meshRenderer.material = _loadedMaterial;
            }

            public void SetNewColor(string colorName)
            {
                if (string.IsNullOrEmpty(colorName)) { return; }

                if (!string.Equals(_interactor.MaterialName, colorName))
                {
                    if (_interactor != null)
                    {
                        _interactor.SetNewMaterials(colorName);
                    }
                }

                Load();
                Set();
            }
        }
    }
}
