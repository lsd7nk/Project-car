using UnityEngine;
using UnityEngine.UI;

public sealed class DisplayTextUpdater : MonoBehaviour
{
    private Text _textField;

    public void Initialize(string value)
    {
        _textField = GetComponent<Text>();
        SetText(value);
    }

    public void Initialize()
    {
        _textField = GetComponent<Text>();
    }

    public void SetText(string value)
    {
        if (_textField == null) { return; }

        _textField.text = value;
    }
}
