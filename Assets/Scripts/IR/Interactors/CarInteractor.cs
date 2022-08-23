using ProjectCar.Repositories;

namespace ProjectCar
{
    namespace Interactors
    {
        public sealed class CarInteractor : Interactor
        {
            private CarRepository _repository;

            public override Repository Repository => _repository;
            public string CarName => _repository.CarName;

            public override void Initialize() => _repository = RepositoryInitialize<CarRepository>();

            public void SetNewCar(string name)
            {
                if (string.IsNullOrEmpty(name)) { return; }

                _repository.SetNewCar(name);
            }
        }
    }
}
