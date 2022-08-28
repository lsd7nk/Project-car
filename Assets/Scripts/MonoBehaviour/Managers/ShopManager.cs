using UnityEngine;
using UnityEngine.UI;
using ProjectCar.Loaders;
using ProjectCar.Interactors;
using ProjectCar.Controllers;

namespace ProjectCar
{
    namespace Managers
    {
        [System.Serializable]
        public sealed class StoreItemButtons
        {
            [SerializeField] private Button _buyButton;
            [SerializeField] private Button _equipButton;

            public Button BuyButton => _buyButton;
            public Button EquipButton => _equipButton;

            public static void SetStateButton(GameObject button, bool state) => button.gameObject.SetActive(state);

            public void TurnOffBuyButton() => _buyButton.gameObject.SetActive(false);

            public void TurnOnEquipButton()
            {
                Image imageButton;

                imageButton = _equipButton.gameObject.GetComponent<Image>();
                imageButton.raycastTarget = true;
                _equipButton.interactable = true;
            }
        }

        public sealed class ShopManager : MonoBehaviour
        {
            [Header("Controller:")]
            [SerializeField] private CoinsController _coinsController;
            [Header("Loaders:")]
            [SerializeField] private CarLoader _carLoader;
            [SerializeField] private MaterialLoader _materialLoader;

            [Header("Color items:")]
            [SerializeField] private StoreItemButtons[] _storeItemButtons;
            private readonly string[] _colorItems = new string[4] { "Blue", "Gray", "Red", "Yellow" };

            [Header("Spoiler items:")]
            [SerializeField] private StoreItemButtons _storeSpoilerItems;

            private ShopItemInteractor _materialInteractor;
            private const int _colorPrice = 35;

            public void BuyMaterial(Material material)
            {
                if (_materialLoader == null) { return; }

                if (_coinsController.BankInteractor.CoinsAmount >= _colorPrice)
                {
                    for (int i = 0; i < _colorItems.Length; ++i)
                    {
                        if (string.Equals(material.name, _colorItems[i]))
                        {
                            _materialInteractor.UnlockItem(material.name);
                            _storeItemButtons[i].TurnOffBuyButton();
                            _storeItemButtons[i].TurnOnEquipButton();

                            _coinsController.BankInteractor.SubsractCoins(_colorPrice);
                        }
                    }
                }
            }

            public void EquipMaterial(Material material)
            {
                if (_materialLoader == null) { return; }

                _materialLoader.SetNewColor(material.name);
            }

            public void EquipSpoiler() => _carLoader.TurnOnSpoiler();

            public void UnequipSpoiler() => _carLoader.TurnOffSpoiler();

            private void Awake()
            {
                InteractorsInitialize();
                StoreItemInitialize();
                StoreSpoilerButtonInititalize();
            }

            private void StoreSpoilerButtonInititalize()
            {
                if (_carLoader == null) { return; }

                if (_carLoader.SpoilerInteractor.CurrentState == "Equipped")
                {
                    StoreItemButtons.SetStateButton(_storeSpoilerItems.BuyButton.gameObject, true);
                    StoreItemButtons.SetStateButton(_storeSpoilerItems.EquipButton.gameObject, false);
                }
                else
                {
                    StoreItemButtons.SetStateButton(_storeSpoilerItems.BuyButton.gameObject, false);
                    StoreItemButtons.SetStateButton(_storeSpoilerItems.EquipButton.gameObject, true);
                }
            }

            private void StoreItemInitialize()
            {
                bool isUnlocked;

                for (int i = 0; i < _colorItems.Length; ++i)
                {
                    isUnlocked = _materialInteractor.CheckStateItem(_colorItems[i]);

                    if (isUnlocked)
                    {
                        _storeItemButtons[i].TurnOffBuyButton();
                        _storeItemButtons[i].TurnOnEquipButton();
                    }
                }
            }

            private void InteractorsInitialize()
            {
                BankInteractor bankInteractor = new BankInteractor();
                _materialInteractor = new ShopItemInteractor();

                bankInteractor.Initialize();
                _materialInteractor.Initialize();
                _coinsController.Initialize(bankInteractor);
            }
        }
    }
}
