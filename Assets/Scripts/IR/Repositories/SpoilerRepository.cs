using UnityEngine;

namespace ProjectCar
{
    namespace Repositories
    {
        public sealed class SpoilerRepository : Repository
        {
            private const string _key = "Spoiler";
            private const string _defaultState = "NotEquipped";
            private const string _equipState = "Equipped";

            public string CurentState { get; private set; }

            public override void Initialize()
            {
                CurentState = PlayerPrefs.HasKey(_key) ? PlayerPrefs.GetString(_key) : _defaultState;
            }

            public void EquipSpoiler()
            {
                CurentState = _equipState;
                Save(_key, CurentState);
            }

            public void UnequipSpoiler()
            {
                CurentState = _defaultState;
                Save(_key, CurentState);
            }
        }
    }
}
