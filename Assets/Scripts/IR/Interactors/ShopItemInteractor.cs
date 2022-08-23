using ProjectCar.Repositories;

namespace ProjectCar
{
    namespace Interactors
    {
        public class ShopItemInteractor : Interactor
        {
            private ShopItemsRepository _repository;

            public override Repository Repository => _repository;

            public override void Initialize() => _repository = RepositoryInitialize<ShopItemsRepository>();

            public void UnlockItem(string key)
            {
                if (string.IsNullOrEmpty(key)) { return; }

                _repository.UnlockItem(key);
            }

            public bool CheckStateItem(string key)
            {
                if (string.IsNullOrEmpty(key)) { return false; }

                return _repository.CheckState(key);
            }
        }
    }
}
