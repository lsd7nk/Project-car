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
        public sealed class ShopItemButtons
        {
            [SerializeField] private Button _buyButton;
            [SerializeField] private Button _equipButton;

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
            [SerializeField] private ShopItemButtons[] _shopItemButtons;
            private readonly string[] _colorItems = new string[4] { "Blue", "Gray", "Red", "Yellow" };

            private ShopItemInteractor _interactor;
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
                            _interactor.UnlockItem(material.name);
                            _shopItemButtons[i].TurnOffBuyButton();
                            _shopItemButtons[i].TurnOnEquipButton();

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

            private void Awake()
            {
                InteractorsInitialize();
                ShopItemInitialize();
            }

            private void ShopItemInitialize()
            {
                bool isUnlocked;

                for (int i = 0; i < _colorItems.Length; ++i)
                {
                    isUnlocked = _interactor.CheckStateItem(_colorItems[i]);

                    if (isUnlocked)
                    {
                        _shopItemButtons[i].TurnOffBuyButton();
                        _shopItemButtons[i].TurnOnEquipButton();
                    }
                }
            }

            private void InteractorsInitialize()
            {
                BankInteractor bankInteractor = new BankInteractor();
                _interactor = new ShopItemInteractor();

                bankInteractor.Initialize();
                _interactor.Initialize();
                _coinsController.Initialize(bankInteractor);
            }
        }
    }
}
