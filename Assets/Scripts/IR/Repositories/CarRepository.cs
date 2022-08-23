using UnityEngine;

namespace ProjectCar
{
    namespace Repositories
    {
        public sealed class CarRepository : Repository
        {
            private const string _key = "CarName";
            private const string _defaultCarName = "Camaro";
            
            public string CarName { get; private set; }

            public override void Initialize()
            {
                CarName = (PlayerPrefs.HasKey(_key)) ? PlayerPrefs.GetString(_key) : _defaultCarName;
            }

            public void SetNewCar(string name)
            {
                CarName = name;
                PlayerPrefs.SetString(_key, CarName);
            }
        }
    }
}