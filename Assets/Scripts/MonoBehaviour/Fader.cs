using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class Fader : MonoBehaviour
{
    private Animator _animator;
    private const string FadeIn = "FadeIn";

    public void Initialize() => _animator = GetComponent<Animator>();

    public void FadeInScreen() => _animator?.SetBool(FadeIn, true);

    public void FadeOutScreen() => _animator?.SetBool(FadeIn, false);
}
