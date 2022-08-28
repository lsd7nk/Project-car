using UnityEngine;

namespace ProjectCar
{
    namespace Repositories
    {
        public abstract class Repository
        {
            public abstract void Initialize();
            protected void Save(string key, int value) => PlayerPrefs.SetInt(key, value);
            protected void Save(string key, string value) => PlayerPrefs.SetString(key, value);
        }
    }
}
