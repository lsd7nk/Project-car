using ProjectCar.Repositories;

namespace ProjectCar
{
    namespace Interactors
    {
        public sealed class CarMaterialInteractor : Interactor
        {
            private CarMaterialRepository _repository;

            public override Repository Repository => _repository;
            public string MaterialName => _repository.MaterialName;

            public override void Initialize() => _repository = RepositoryInitialize<CarMaterialRepository>();

            public void SetNewMaterials(string name)
            {
                if (string.IsNullOrEmpty(name)) { return; }

                _repository.SetNewMaterial(name);
            }
        }
    }
}
