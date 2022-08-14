using UnityEngine;
using TMPro;

namespace ProjectCar
{
    namespace UI
    {
        [RequireComponent(typeof(TextMeshProUGUI))]
        public sealed class DisplayTextUpdater : MonoBehaviour
        {
            private TextMeshProUGUI _textField;

            public void Initialize(string value)
            {
                Initialize();
                SetText(value);
            }

            public void Initialize() => _textField = GetComponent<TextMeshProUGUI>();

            public void SetText(string value)
            {
                if (_textField == null) { return; }

                _textField.text = value;
            }
        }
    }
}