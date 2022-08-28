using ProjectCar.Repositories;

namespace ProjectCar
{
    namespace Interactors
    {
        public class SpoilerInteractor : Interactor
        {
            private SpoilerRepository _repository;

            public override Repository Repository => _repository;
            public string CurrentState => _repository.CurentState;

            public override void Initialize() => _repository = RepositoryInitialize<SpoilerRepository>();

            public void EquipSpoiler() => _repository.EquipSpoiler();

            public void UnequipSpoiler() => _repository.UnequipSpoiler();
        }
    }
}
