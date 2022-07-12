using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Fader : MonoBehaviour
{
    private Animator _animator;
    private const string FadeIn = "FadeIn";

    public void Initialize() => _animator = GetComponent<Animator>();

    public void FadeInScreen()
    {
        if (_animator == null) { return; }

        _animator.enabled = true;
        _animator?.SetBool(FadeIn, true);
    }

    public void FadeOutScreen() => _animator?.SetBool(FadeIn, false);

    public void DisableAnimator()
    {
        if (_animator == null) { return; }

        _animator.enabled = false;
    }
}
