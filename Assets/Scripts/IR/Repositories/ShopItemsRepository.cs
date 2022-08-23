using System.Collections;
using UnityEngine;

namespace ProjectCar
{
    namespace Repositories
    {
        public sealed class ShopItemsRepository : Repository
        {
            private readonly string[] _keys = new string[4] { "Blue", "Gray", "Red", "Yellow" };
            private readonly string[] _itemsStates = new string[4];
            private const string _defaultState = "Locked";
            private const string _unlockedState = "Unlocked";

            public IEnumerable this[int index] => _itemsStates[index];

            public override void Initialize()
            {
                for (int i = 0; i < _keys.Length; ++i)
                {
                    _itemsStates[i] = (PlayerPrefs.HasKey(_keys[i])) ? PlayerPrefs.GetString(_keys[i]) : _defaultState;
                }
            }

            public void UnlockItem(string key)
            {
                for (int i = 0; i < _keys.Length; ++i)
                {
                    if (string.Equals(_keys[i], key))
                    {
                        _itemsStates[i] = _unlockedState;
                        PlayerPrefs.SetString(key, _unlockedState);
                    }
                }
            }

            public bool CheckState(string key)
            {
                for (int i = 0; i < _keys.Length; ++i)
                {
                    if (string.Equals(_keys[i], key))
                    {
                        if (_itemsStates[i] == _unlockedState)
                        {
                            return true;
                        }
                    }
                }
                
                return false;
            }
        }
    }
}
