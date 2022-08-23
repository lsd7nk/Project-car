using UnityEngine;

namespace ProjectCar
{
    namespace Repositories
    {
        public sealed class CarMaterialRepository : Repository
        {
            private const string _key = "MaterialName";
            private const string _defaultCarName = "Purple";

            public string MaterialName { get; private set; }

            public override void Initialize()
            {
                MaterialName = (PlayerPrefs.HasKey(_key)) ? PlayerPrefs.GetString(_key) : _defaultCarName;
            }

            public void SetNewMaterial(string name)
            {
                MaterialName = name;
                PlayerPrefs.SetString(_key, MaterialName);
            }
        }
    }
}
